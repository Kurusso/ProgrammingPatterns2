using Common.Helpers;
using CoreApplication.Models;
using CoreApplication.Models.CurrencyApi;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;

namespace CoreApplication.BackgroundJobs
{
    public class CurrencyJob : IJob
    {
        private readonly string _getCurrency;
        private readonly HttpClient _httpClient;
        private readonly string _token;
        public CurrencyJob(IConfiguration configuration)
        {
            var coreSection = configuration.GetSection("CurrencyApi");
            _getCurrency = coreSection["GetCurrency"];
            _token = coreSection["Token"];
            _httpClient = new HttpClient();
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var response = await _httpClient.GetAsync(_getCurrency + "?apikey=" + _token + "&currencies=EUR%2CUSD%2CRUB");
            response.EnsureSuccessStatusCode();
            var responseMsg = await response.Content.ReadAsStringAsync();
            try
            {
                var result = JsonConvert.DeserializeObject<CurrencyResponseDTO>(responseMsg);
                CurrencyValues.Instance.UpdateValues(result.Data.RUB.Value, result.Data.EUR.Value, result.Data.USD.Value);
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}