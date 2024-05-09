using Common.BackgroundJobs;
using Common.Helpers.StartupServiceConfigurator;
using Quartz;

namespace CoreApplication.BackgroundJobs
{
    public static class DataScheduler
    {

        public static void RegisterBackgroundJobs(this WebApplicationBuilder? builder, IConfiguration serviceProvider, QuartzConfigurator quartzConfigurer)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            var quartzSection = serviceProvider.GetSection("CurrencyJob");
            var interval = quartzSection["IntervalInMinutes"];
            int _interval = int.Parse(interval);
            quartzConfigurer.Append(q =>
            {
                q.AddJob<CurrencyJob>(opts => opts.WithIdentity(nameof(CurrencyJob)));
                q.AddTrigger(opts => opts
                .ForJob(nameof(CurrencyJob))
                .WithIdentity($"{nameof(CurrencyJob)}Trigger", $"{nameof(CurrencyJob)}Group")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(_interval)
                    .RepeatForever()));
            });
        }
    }
}
