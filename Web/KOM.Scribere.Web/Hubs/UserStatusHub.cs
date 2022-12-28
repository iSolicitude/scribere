using KOM.Scribere.Data.Common.Repositories;
using KOM.Scribere.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace KOM.Scribere.Web.Hubs;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

public class UserStatusHub : Hub
{
    private static readonly ICollection<string> OnlineUsers = new HashSet<string>();

    private readonly IDeletableEntityRepository<User> usersRepository;

    public UserStatusHub(IDeletableEntityRepository<User> usersRepository)
    {
        this.usersRepository = usersRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var username = this.Context.User.Identity.Name;

        if (username != null)
        {
            OnlineUsers.Add(username);

            await this.Clients.All.SendAsync("IsUserOnline", username);
            await base.OnConnectedAsync();
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var username = this.Context.User.Identity.Name;

        if (username != null)
        {
            OnlineUsers.Remove(username);

            await this.Clients.All.SendAsync("UserIsOffline", username);
            await base.OnDisconnectedAsync(exception);
        }
    }

    public async Task IsUserOnline(string username)
    {
        var isExisting = await this.usersRepository
            .AllAsNoTracking()
            .AnyAsync(u => u.UserName == username);

        if (isExisting == true)
        {
            if (OnlineUsers.Contains(username))
            {
                await this.Clients.All.SendAsync("UserIsOnline", username);
            }
            else
            {
                await this.Clients.All.SendAsync("UserIsOffline", username);
            }
        }
    }
}