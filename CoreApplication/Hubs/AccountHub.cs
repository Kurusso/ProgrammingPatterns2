using CoreApplication.Models.DTO;
using Microsoft.AspNetCore.SignalR;

namespace CoreApplication.Hubs;

public class AccountHub:Hub
{
    public async Task SendAccountInfo(AccountDTO accountInfo)
    {
        Console.WriteLine("SendingAccountInfo");
        await Clients.All.SendAsync("ReceiveAccountInfo", accountInfo);
        
    }
}