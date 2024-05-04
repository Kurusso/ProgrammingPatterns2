using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using CoreApplication.Hubs;
using Microsoft.AspNetCore.SignalR;
using CoreApplication.Models;
using CoreApplication.Models.DTO;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.Text;
using Common.Models.Enumeration;
namespace CoreApplication.Helpers
{

    public static class OperationUpdateHelper
    {
        private static string CurrencyIcon(Currency c) {
            return c switch
            {
                Currency.Ruble => "₽",
                Currency.Dollar => "$",
                Currency.Euro => "€",
                _ => "",
            };
        }
        private static string FormatOperation(Operation op) {
            return $"account {op.AccountId}.\n{op.MoneyAmmount.Amount}{CurrencyIcon(op.MoneyAmmount.Currency)}";
        }

        public static async Task CatchOperationUpdate(ChangeTracker changeTracker, IHubContext<ClientOperationsHub> _hubContext, CoreDbContext context, CustomWebSocketManager webSocketManager, HttpClient firebaseClient, IConfiguration configuration)
        {
            var modifiedEntries = changeTracker.Entries()
            .Where(e => e.State == EntityState.Added && e.Entity.GetType() == typeof(Operation))
            .Select(e => e.Entity)
            .ToList();
            var firebase = configuration.GetSection("Firebase");
            var firebaseUrl = firebase.GetValue<string>("FcmUrl");
            var userAppId = firebase.GetValue<string>("UserAppId");
            var staffAppId = firebase.GetValue<string>("StaffAppId");
            List<Account> accounts = new List<Account>();
            foreach (var entity in modifiedEntries)
            {
                accounts.Add(((Operation)entity).Account);
                var userDevicesForSend = await context.DeviceTokens.Where(x => x.UserId == ((Operation)entity).Account.UserId && x.AppId== userAppId).ToListAsync();
                var staffDevicesForSend = await context.DeviceTokens.Where(x=>x.AppId==staffAppId).ToListAsync();
                foreach (var device in userDevicesForSend)
                {
                    await SendNotificationToDeviceAsync(device.Token, ((Operation)entity).OperationType.ToString(), JsonConvert.SerializeObject(new OperationWithUserIdDTO((Operation)entity)), firebaseClient, firebaseUrl);
                }
                foreach (var device in staffDevicesForSend)
                {
                    var notifBody = FormatOperation((Operation)entity);
                    await SendNotificationToDeviceAsync(device.Token, ((Operation)entity).OperationType.ToString(), notifBody, firebaseClient, firebaseUrl);
                }
            }
            accounts = accounts.Distinct().ToList();
            foreach (var account in accounts)
            {
                var accountForSend = await context.Accounts.Include(x => x.Operations).FirstOrDefaultAsync(x => x.Id == account.Id);        

                string json = JsonConvert.SerializeObject(new AccountDTO(accountForSend));
                await _hubContext.Clients.All.SendAsync("ReceiveAccount", new AccountDTO(accountForSend));
                try
                {
                    await webSocketManager.SendMessageToUser(accountForSend.UserId.ToString(), json);
                }
                catch { }
                //await _hubContext.Clients.User(accountForSend.UserId.ToString()).SendAsync("ReceiveAccount", new AccountDTO(accountForSend));
            }

        }
        private static async Task SendNotificationToDeviceAsync(string deviceToken, string operationType, string operation, HttpClient client, string url)
        {
            var payload = new
            {
                to = deviceToken,
                notification = new
                {
                    title = operationType,
                    body = operation
                }
            };

            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var httpResponse = await client.PostAsync(url, httpContent);
        }
    }
}
