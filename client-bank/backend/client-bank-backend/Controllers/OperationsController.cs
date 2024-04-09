using System.Transactions;
using client_bank_backend.Heplers;
using client_bank_backend.Services.RabbitMqServices;
using Common.Models.Dto;
using Common.Models.Enumeration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace client_bank_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OperationsController : ControllerBase
{
    private readonly IRabbitMqService _rabbitMqOperationService;
    private readonly HttpClient _coreClient = new();

    public OperationsController(IRabbitMqService rabbitMqOperationService)
    {
        _rabbitMqOperationService = rabbitMqOperationService;
    }

    [HttpPost]
    [Route("Deposit")]
    public async Task<IActionResult> Deposit(Guid accountId, int money, Currency currency)
    {
        try
        {
            var userId = await AuthHelper.Validate(_coreClient, Request);
            if (userId.IsNullOrEmpty()) return Unauthorized();
            await QueueOperation(accountId, currency, new Guid(userId), money, OperationType.Deposit, null);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while creating the account.");
        }
    }

    [HttpPost]
    [Route("Withdraw")]
    public async Task<IActionResult> Withdraw(Guid accountId, int money, Currency currency)
    {
        try
        {
            var userId = await AuthHelper.Validate(_coreClient, Request);
            if (userId.IsNullOrEmpty()) return Unauthorized();
            await QueueOperation(accountId, currency, new Guid(userId), money, OperationType.Withdraw, null);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while creating the account.");
        }
    }

    [HttpPost]
    [Route("transfer")]
    public async Task<IActionResult> Transfer(Guid accountId, decimal money, Currency currency, Guid reciveAccountId)
    {
        try
        {
            var userId = await AuthHelper.Validate(_coreClient, Request);
            if (userId.IsNullOrEmpty()) return Unauthorized();
            await QueueOperation(accountId, currency, new Guid(userId), money, OperationType.TransferSend,
                reciveAccountId);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task QueueOperation(Guid accountId, Currency currency, Guid userId, decimal money,
        OperationType operationType, Guid? reciveAccountId)
    {
        var trackingId = Guid.NewGuid();
        var tracker = new ScopedConfirmationMessageFeedbackTracker();
        tracker.Track(trackingId.ToString());
        _rabbitMqOperationService.SendMessage(new OperationPostDTO
        {
            Id = trackingId,
            AccountId = accountId,
            Currency = currency,
            UserId = userId,
            MoneyAmmount = money,
            RecieverAccount = reciveAccountId,
            OperationType = operationType,
        });
        await tracker.WaitFor(trackingId.ToString(), TimeSpan.FromSeconds(10));
        var message = tracker.Get(trackingId.ToString())!;
        if (message.Status != 200)
        {
            if (message.Status == 400)
            {
                throw new InvalidOperationException(message.Message);
            }

            throw new TransactionException(message.Message);
        }
    }
}