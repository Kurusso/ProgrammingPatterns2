using CreditApplication.Services;
using Quartz;

namespace CreditApplication.Quartz.Jobs
{
    public class CreditJob : IJob
    {
        private readonly ICreditService _creditService;
        public CreditJob(ICreditService creditService)
        {
            _creditService = creditService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await _creditService.UpdateCredits();
        }
    }
}
