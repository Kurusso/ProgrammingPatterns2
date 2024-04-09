using System.Net.Http.Headers;
using client_bank_backend.DTOs;
using client_bank_backend.Hubs;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;

namespace client_bank_backend.Services;

public class AccountHubService:IHostedService
{
    private readonly IHubContext<BffAccountHub> _bffAccountHubContext;
    private HubConnection _backendHubConnection;

    public AccountHubService(IHubContext<BffAccountHub> bffAccountHubContext)
    {
        _bffAccountHubContext = bffAccountHubContext;
    }

    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _backendHubConnection = new HubConnectionBuilder()
            .WithUrl(MagicConstants.AccountHub)
            .Build();

        _backendHubConnection.On<AccountDTO>("ReceiveAccount", 
            
            (accountInfo) =>
        {
            ForwardAccountInfoToClients(accountInfo);
        });

        return _backendHubConnection.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _backendHubConnection?.DisposeAsync().AsTask() ?? Task.CompletedTask;
    }

    private void ForwardAccountInfoToClients(AccountDTO accountInfo)
    {
        // Get connections from map
        if (BffAccountHub._userConnectionMap.TryGetValue(accountInfo.UserId.ToString(), out List<string> connectionsId)&&connectionsId.Any())
        {
            _bffAccountHubContext.Clients.Clients(connectionsId).SendAsync("ReceiveAccount", accountInfo);
        }
    }
}