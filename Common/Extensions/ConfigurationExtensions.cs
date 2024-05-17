using Common.Helpers.StartupServiceConfigurator;
using Common.Jobs;
using Common.Middlewares;
using Common.Services;
using Destructurama;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;
using Serilog.Formatting.Compact;
using System.Configuration;
using System.Net.Sockets;

namespace Common.Extensions
{
    public static partial class ConfigurationExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sourceConfiguration">IConfiguration source containing logger configuration. If not specified <paramref name="builder"/> configuration is used</param>
        /// <param name="loggerConfigurationSectionName">section name for logger configuration</param>
        public static void AddLogCollection(this WebApplicationBuilder builder, IConfiguration? sourceConfiguration = null, string? loggerConfigurationSectionName = null)
        {
            builder.Host.UseSerilog((context, loggerConfiguration) =>
            {
                if (sourceConfiguration is not null || loggerConfigurationSectionName is not null)
                {
                    var conf = sourceConfiguration ?? builder.Configuration;
                    if (loggerConfigurationSectionName is not null)
                    {
                        loggerConfiguration.ReadFrom.Configuration(conf, readerOptions: new Serilog.Settings.Configuration.ConfigurationReaderOptions
                        {
                            SectionName = loggerConfigurationSectionName
                        });
                    }
                    else
                    {
                        loggerConfiguration.ReadFrom.Configuration(conf);
                    }
                    return;
                }

                //Default Configuration
                loggerConfiguration.MinimumLevel.Information();
                loggerConfiguration.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning);
                loggerConfiguration.MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", Serilog.Events.LogEventLevel.Error);
                loggerConfiguration.MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Error);

                loggerConfiguration.WriteTo.File(new CompactJsonFormatter(),
                                                 "./logs/log.txt",
                                                 rollingInterval: RollingInterval.Minute
                //rollOnFileSizeLimit: true,
                //fileSizeLimitBytes: 60000
                ).Enrich.WithProperty("Service", System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name)
                .WriteTo.Console()
                .Destructure.JsonNetTypes();
            });
        }

        public static void RegisterLogPublishingJobs(this WebApplicationBuilder builder, QuartzConfigurator quartzConfigurer, IConfiguration? configuration = null)
        {
            configuration ??= builder.Configuration;
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            int.TryParse(configuration["LogCollection:IntervalSeconds"], out var interval);

            quartzConfigurer.Append(q =>
            {
                q.AddJob<LogPublishingJob>(opts => opts.WithIdentity(nameof(LogPublishingJob)));
                q.AddTrigger(opts => opts
                    .ForJob(nameof(LogPublishingJob))
                    .WithIdentity($"{nameof(LogPublishingJob)}Trigger", $"{nameof(LogPublishingJob)}Group")
                .StartNow()
                .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(interval)
                .RepeatForever()));
            });

            //Multiple registration resolved by ASP.Net Core in favor of the last call, however all cases are registered and tracked.
            //builder.Services.AddQuartzHostedService(x => x.WaitForJobsToComplete = true);
        }

        public static void AddQuartzConfigured(this WebApplicationBuilder builder, QuartzConfigurator configurer)
        {
            builder.Services.AddQuartz(configurer.Configure);
            builder.Services.AddQuartzHostedService(x => x.WaitForJobsToComplete = true);
        }

        public static void UseTracingMiddleware(this WebApplication app)
        {
            app.UseMiddleware<TracingMiddleware>();
        }

        public static void RegisterInternalHttpClientDeps(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }
    }
}
