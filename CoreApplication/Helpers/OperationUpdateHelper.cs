﻿using Common.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using CoreApplication.Hubs;
using Microsoft.AspNetCore.SignalR;
using CoreApplication.Models;
using CoreApplication.Models.DTO;
namespace CoreApplication.Helpers
{

    public static class OperationUpdateHelper
    {
        public static async Task CatchOperationUpdate(ChangeTracker changeTracker, IHubContext<ClientOperationsHub> _hubContext, CoreDbContext context)
        {
            var modifiedEntries = changeTracker.Entries()
            .Where(e => e.State == EntityState.Added && e.Entity.GetType() == typeof(Operation))
            .Select(e => e.Entity)
            .ToList();
            List<Account> accounts = new List<Account>();
            foreach (var entity in modifiedEntries)
            {
                accounts.Add(((Operation)entity).Account);
            }
            accounts = accounts.Distinct().ToList();
            foreach (var account in accounts)
            {
                var accountForSend = await context.Accounts.Include(x => x.Operations).FirstOrDefaultAsync(x => x.Id == account.Id);
                // await _hubContext.Clients.User(accountForSend.UserId.ToString()).SendAsync("/api/client", new AccountDTO(accountForSend));
                await _hubContext.Clients.All.SendAsync("/api/client", new AccountDTO(accountForSend));
            }

        }
    }
}