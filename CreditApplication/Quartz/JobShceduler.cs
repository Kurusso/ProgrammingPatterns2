using CreditApplication.Quartz.Jobs;
using Quartz;

namespace CreditApplication.Quartz
{
    public static class JobShceduler
    {
        public static void RegisterBackgroundJobs(this WebApplicationBuilder? builder, IConfiguration serviceProvider)
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
            builder.Services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                q.AddJob<CreditJob>(opts => opts.WithIdentity(nameof(CreditJob)));
                q.AddTrigger(opts => opts
                .ForJob(nameof(CreditJob))
                .WithIdentity($"{nameof(CreditJob)}Trigger", $"{nameof(CreditJob)}Group")
                .StartAt(DateTime.Now.Date.Add(timeDateTimeFormat))
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(_interval)
                    .RepeatForever()));

               
            });

            builder.Services.AddQuartzHostedService(x => x.WaitForJobsToComplete = true);
        }
    }
}
