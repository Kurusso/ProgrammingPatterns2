using Common.Models.Dto;
using CreditApplication.Models;
using CreditApplication.Models.Dtos;
using CreditApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using CreditRateDTO = CreditApplication.Models.DTOs.CreditRateDTO;

namespace CreditApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditRatesController : ControllerBase
    {
        private readonly ICreditRateService _creditRateService;
        public CreditRatesController (ICreditRateService creditRateService)
        {
            _creditRateService = creditRateService;
        }

        [HttpPost]
        [Route("Create")]
        public async Task <IActionResult> CreateCreditRate(CreditRateDTO creditRate)
        {
            try
            {
                await _creditRateService.CreateCreditRate(creditRate);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return Problem(statusCode: 401, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetCreditRates()
        {
            try
            {
                var rates = await _creditRateService.GetAllCreditRates();
                return Ok(rates);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: ex.Message);
            }
        }
    }

}
