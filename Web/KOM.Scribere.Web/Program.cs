using System.Globalization;
using KOM.Scribere.Services.Data.Users;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;

namespace KOM.Scribere.Web;

using System.Reflection;

using KOM.Scribere.Data;
using KOM.Scribere.Data.Common;
using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using KOM.Scribere.Data.Repositories;
using KOM.Scribere.Data.Seeding;
using KOM.Scribere.Services.Data;
using KOM.Scribere.Services.Mapping;
using KOM.Scribere.Services.Messaging;
using KOM.Scribere.Web.ViewModels;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services, builder.Configuration);
        var app = builder.Build();
        Configure(app);
        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddDefaultIdentity<User>(IdentityOptionsProvider.GetIdentityOptions)
            .AddRoles<Role>().AddEntityFrameworkStores<ApplicationDbContext>();
        
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.AddMvc()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("bg"),
            };

            options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        services.Configure<CookiePolicyOptions>(
            options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

        services.Configure<CookieTempDataProviderOptions>(options =>
        {
            options.Cookie.IsEssential = true;
        });

        services.ConfigureApplicationCookie(options =>
        {
            options.AccessDeniedPath = "/Error403";
            options.Cookie.HttpOnly = true;
            options.LoginPath = "/identity/account/login";
            options.LogoutPath = "/logout";
        });

        services.AddControllersWithViews(
            options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }).AddRazorRuntimeCompilation();
        services.AddRazorPages();
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddSingleton(configuration);

        // Data repositories
        services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IDbQueryRunner, DbQueryRunner>();

        // Application services
        services.AddTransient<IEmailSender, NullMessageSender>();
        services.AddTransient<IEmailSender>(x => new SendGridEmailSender(configuration["SendGrid:ApiKey"]));
        services.AddTransient<ISettingsService, SettingsService>();
        services.AddTransient<IUsersService, UsersService>();

    }

    private static void Configure(WebApplication app)
    {
        // Seed data on application startup
        using (var serviceScope = app.Services.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
            new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
        }

        AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
        app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();
    }
}
