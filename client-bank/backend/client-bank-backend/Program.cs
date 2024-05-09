using client_bank_backend.Hubs;
using client_bank_backend.Services;
using client_bank_backend.Services.RabbitMqServices;
using Common.Extensions;
using Common.Helpers;
using Common.Helpers.StartupServiceConfigurator;
using Common.Services;

var builder = WebApplication.CreateBuilder(args);
var quartzConfigurator = new QuartzConfigurator();

var services = builder.Services;

services.AddSingleton<IHostedService, AccountHubService>();
services.AddScoped<IRabbitMqService, RabbitMQIntegrationService>();
//services.AddHostedService< RabbitMQFeedbackListener>();

builder.AddLogCollection();
builder.RegisterLogPublishingJobs(quartzConfigurator);
builder.AddQuartzConfigured(quartzConfigurator);

builder.RegisterInternalHttpClientDeps();
builder.AddIdempotentAutoRetryHttpClient<TracingHttpClient>();

services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            corsPolicyBuilder
                .WithOrigins("https://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
services.AddSignalR();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();
app.UseTracingMiddleware();

app.MapHub<BffAccountHub>("/AccountHub");
app.MapControllers();

app.Run();