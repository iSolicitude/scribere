using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;

using CloudinaryDotNet;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using KOM.Scribere.Data;
using KOM.Scribere.Data.Common;
using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Data.Repositories;
using KOM.Scribere.Data.Seeding;
using KOM.Scribere.Services;
using KOM.Scribere.Services.Data;
using KOM.Scribere.Services.Data.Notifications;
using KOM.Scribere.Services.Data.Profile;
using KOM.Scribere.Services.Data.UserPenalties;
using KOM.Scribere.Services.Data.Users;
using KOM.Scribere.Services.Mapping;
using KOM.Scribere.Services.Messaging;
using KOM.Scribere.Web.Extensions;
using KOM.Scribere.Web.Hubs;
using KOM.Scribere.Web.Infrastructure.CronJobs;
using KOM.Scribere.Web.Infrastructure.Hangfire;
using KOM.Scribere.Web.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Serilog;
using Serilog.Events;
using Serilog.Ui.MsSqlServerProvider;
using Serilog.Ui.Web;
using WebEssentials.AspNetCore.OutputCaching;
using WebEssentials.AspNetCore.Pwa;
using WebMarkupMin.AspNetCore7;
using WebMarkupMin.Core;
using WebMarkupMin.Core.Loggers;
using WilderMinds.MetaWeblog;

using ILogger = WebMarkupMin.Core.Loggers.ILogger;
using MetaWeblogService = KOM.Scribere.Services.Data.MetaWeblogService;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddHangfire(
    config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                UsePageLocksOnDequeue = true,
                DisableGlobalLocks = true,
            }).UseConsole());

builder.Services.AddSerilogUi(options => options
    .EnableAuthorization(authOptions =>
    {
        authOptions.AuthenticationType = AuthenticationType.Cookie; // or AuthenticationType.Cookie
        authOptions.Usernames = new[] { "kingyadid", "User2" };
        authOptions.Roles = new[] { "Administration" };
    })
    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), "Scribere Logs"));

builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo("DataEncrpytionKeys"));
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDefaultIdentity<User>(IdentityOptionsProvider.GetIdentityOptions)
    .AddRoles<Role>()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"c:\temp-keys\"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("bg"),
    };

    options.DefaultRequestCulture = new RequestCulture("en", "en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.Configure<CookieTempDataProviderOptions>(options => { options.Cookie.IsEssential = true; });
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.Configure<SecurityStampValidatorOptions>(options => { options.ValidationInterval = TimeSpan.Zero; });
builder.Services.Configure<BlogSettings>(builder.Configuration.GetSection("BlogSettings"));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Error403";
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/identity/account/login";
    options.LogoutPath = "/logout";
});

builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    })
    .AddViewLocalization(options => options.ResourcesPath = "Resources")
    .AddRazorRuntimeCompilation();

builder.Services.AddRazorPages();
builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();
builder.Services.AddAntiforgery(options => { options.HeaderName = "X-CSRF-TOKEN"; });

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        var googleAuthNSection = builder.Configuration.GetSection("Google");
        options.ClientId = googleAuthNSection["ClientId"]!;
        options.ClientSecret = googleAuthNSection["ClientSecret"]!;
    })
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["FacebookSettings:AppId"]!;
        options.AppSecret = builder.Configuration["FacebookSettings:AppSecret"]!;
        options.AccessDeniedPath = "/AccessDeniedPathInfo";
    });

builder.Services.AddProgressiveWebApp(
    new PwaOptions
    {
        OfflineRoute = "/shared/offline/"
    });

// Output caching (https://github.com/madskristensen/WebEssentials.AspNetCore.OutputCaching)
builder.Services.AddOutputCaching(options =>
{
    options.Profiles["default"] = new OutputCacheProfile
    {
        Duration = 3600
    };
});

builder.Services.AddWebMarkupMin(options =>
    {
        options.AllowMinificationInDevelopmentEnvironment = true;
        options.DisablePoweredByHttpHeaders = true;
    })
    .AddHtmlMinification(options =>
    {
        options.MinificationSettings.RemoveOptionalEndTags = false;
        options.MinificationSettings.WhitespaceMinificationMode = WhitespaceMinificationMode.Safe;
    });

// Bundling, minification and Sass transpilation (https://github.com/ligershark/WebOptimizer)
builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.MinifyJsFiles();
    pipeline.CompileScssFiles()
        .InlineImages(1);
});

builder.Services.AddSignalR();

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression(opt => opt.EnableForHttps = true);

builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddHangfireServer(options => options.WorkerCount = 2);

var account = new Account(
    builder.Configuration["Cloudinary:CloudName"],
    builder.Configuration["Cloudinary:ApiKey"],
    builder.Configuration["Cloudinary:ApiSecret"]);

var cloudinary = new Cloudinary(account);

builder.Services.AddSingleton(cloudinary);

// Data repositories
builder.Services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IDbQueryRunner, DbQueryRunner>();

// Application services
builder.Services.AddTransient<IEmailSender>(x => new SendGridEmailSender(builder.Configuration["SendGrid:ApiKey"]));

builder.Services.AddTransient<ICloudinaryService, CloudinaryService>();
builder.Services.AddTransient<IEmailSender, NullMessageSender>();
builder.Services.AddTransient<IBlogService, FileExtension>();
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IProfileService, ProfileService>();
builder.Services.AddTransient<IUserPenaltiesService, UserPenaltiesService>();

builder.Services.AddTransient<ISettingsService, SettingsService>();
builder.Services.AddTransient<INotificationsService, NotificationsService>();

MetaWeblogExtensions.AddMetaWeblog<MetaWeblogService>(builder.Services);

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSingleton<ILogger, NullLogger>();

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider)
        .GetAwaiter()
        .GetResult();
}

AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

var recurringJobManager = app.Services.GetService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate<CheckFeedsJob>(nameof(CheckFeedsJob), x => x.Work(null), "*/1 * * * *");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("{0}");
    app.UseHsts();
}

app.Use(
    (context, next) =>
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        return next();
    });

app.UseResponseCompression();

app.UseStatusCodePages();
app.UseStatusCodePagesWithReExecute("/Shared/Error");

if (builder.Configuration.GetValue<bool>("forcessl")) app.UseHttpsRedirection();

MetaWeblogExtensions.UseMetaWeblog(app, "/metaweblog");

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 24 * 60 * 60; // 24 hours
        ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;
    },
});
app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();

app.UseOutputCaching();
app.UseWebMarkupMin();
app.UseAuthorization();

app.UseSerilogRequestLogging();
app.UseSerilogUi(options => { options.HomeUrl = "/"; });

app.UseHangfireDashboard(options: new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() },
    DashboardTitle = "Scribere Dashboard",
    AppPath = "/hangfire"
});

if (app.Environment.IsProduction())
{
    builder.Services.AddHangfireServer(options => options.WorkerCount = 2);
}

app.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<UserStatusHub>("/userStatusHub");

app.MapRazorPages();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", LogEventLevel.Error)
    .Enrich.FromLogContext()
    .WriteTo.Console(LogEventLevel.Debug)
    .WriteTo.Console(LogEventLevel.Error)
    .WriteTo.RollingFile("wwwroot/Logs/Log-{Date}.txt",
        outputTemplate:
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}")
    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Error).WriteTo.RollingFile(
        "wwwroot/Logs/Errors/ErrorLog-{Date}.txt",
        outputTemplate:
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}"))
    .CreateLogger();

try
{
    Log.Information("Starting application");
    app.Run();
    Log.Information("Stopped application");
    return 0;
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
