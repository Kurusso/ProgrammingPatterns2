using Common.BackgroundJobs;
using Common.Extensions;
using CreditApplication.Quartz.Jobs;
using Quartz;

namespace CreditApplication.Quartz
{
    public static class JobShceduler
    {
        public static void RegisterBackgroundJobs(this WebApplicationBuilder? builder, IConfiguration serviceProvider, QuartzConfigurator quartzConfigurer)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            var quartzSection = serviceProvider.GetSection("Quartz");
            var CreditJobInfo = quartzSection.GetSection("CreditJob");
            var time = CreditJobInfo["StartingTime"];
            var interval = CreditJobInfo["IntervalInMinutes"];
            TimeSpan timeDateTimeFormat = TimeSpan.Parse(time);
            int _interval = int.Parse(interval);
            quartzConfigurer.Append(q =>
            {
                q.AddJob<CreditJob>(opts => opts.WithIdentity(nameof(CreditJob)));
                q.AddTrigger(opts => opts
                .ForJob(nameof(CreditJob))
                .WithIdentity($"{nameof(CreditJob)}Trigger", $"{nameof(CreditJob)}Group")
                .StartAt(DateTime.Now.Date.Add(timeDateTimeFormat))
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(_interval)
                    .RepeatForever()));
            });

            var quartzSection2 = serviceProvider.GetSection("CurrencyJob");
            var interval2 = quartzSection["IntervalInMinutes"];
            int _interval2 = int.Parse(interval);
            quartzConfigurer.Append(q =>
            {
                q.AddJob<CurrencyJob>(opts => opts.WithIdentity(nameof(CurrencyJob)));
                q.AddTrigger(opts => opts
                .ForJob(nameof(CurrencyJob))
                .WithIdentity($"{nameof(CurrencyJob)}Trigger", $"{nameof(CurrencyJob)}Group")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(_interval2)
                    .RepeatForever()));
            });
        }
    }
}
