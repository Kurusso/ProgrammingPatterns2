using Common.Models.Enumeration;
using CreditApplication.Models;
using CreditApplication.Models.Dtos;
using CreditApplication.Models.DTOs;
using CreditApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Transactions;

namespace CreditApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PenaltyController : ControllerBase
    {
        private readonly ICreditPenaltyService _creditPenaltyService;
        public PenaltyController(ICreditPenaltyService creditPenaltyService)
        {
            _creditPenaltyService = creditPenaltyService;
        }

        [HttpGet]
        [Route("GetUserPenalties")]
        public async Task<ActionResult<ICollection<PenaltyDTO>>> GetUserPenalties(Guid userId)
        {
            try
            {
                var penalties = await _creditPenaltyService.GetUserPenalties(userId);
                return Ok(penalties);
            }
            catch (ArgumentException ex)
            {
                return Problem(statusCode: 400, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCreditPenalties")]
        public async Task<ActionResult<ICollection<PenaltyDTO>>> GetCreditPenalties(Guid creditId, Guid userId)
        {
            try
            {
                var penalties = await _creditPenaltyService.GetCreditPenalties(creditId, userId);
                return Ok(penalties);
            }
            catch(ArgumentException ex)
            {
                return Problem(statusCode: 400, detail: ex.Message);
            }
            catch(KeyNotFoundException ex)
            {
                return Problem(statusCode: 404, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: ex.Message);
            }
        }

        [HttpPost]
        [Route("Repay")]
        public async Task<IActionResult> RepayPenalty(Guid id, Guid userId, int moneyAmmount, Currency currency, Guid? accountId = null)
        {
            try
            {
                await _creditPenaltyService.RepayPenalty(id, userId, moneyAmmount, accountId, currency);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return Problem(statusCode: 404, detail: ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Problem(statusCode: 401, detail: ex.Message);
            }
            catch (TransactionException)
            {
                return Problem(statusCode: 401, detail: nameof(TransactionException));
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: ex.Message);
            }
        }
    }
}
