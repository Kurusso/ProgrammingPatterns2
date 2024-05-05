using Common.Helpers;
using Common.Models;
using Common.Middleware;
using Common.Extensions;
using CoreApplication.BackgroundJobs;
using CoreApplication.Configurations;
using CoreApplication.Hubs;
using CoreApplication.Initialization;
using CoreApplication.Models;
using CoreApplication.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;
var quartzConfigurator = new QuartzConfigurator();
builder.AddLogCollection();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddScoped<IAccountService, AccountService>();
services.AddScoped<IMoneyOperationsService, MoneyOperationsService>();
services.AddScoped<IUserService, UserService>();
services.AddSwaggerGen(c => {
    c.OperationFilter<ExposeIdempotentIdSwaggerFilter>();
});
services.AddSignalR();
services.AddSingleton<CustomWebSocketManager>();
services.AddHostedService<OperationsListener>();

builder.AddDB<CoreDbContext>("DefaultConnection");
builder.AddIdempotenceDB("IdempotenceDbConnection");

var notificationSettings = builder.Configuration.GetSection("RabbitMqConfigurations").Get<RabbitMqConfigurations>();
builder.Services.Configure<RabbitMqConfigurations>(builder.Configuration.GetSection("RabbitMqConfigurations"));
builder.RegisterBackgroundJobs(configuration, quartzConfigurator);
builder.RegisterLogPublishingJobs(quartzConfigurator);
builder.AddQuartzConfigured(quartzConfigurator);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseTracingMiddleware();
app.MigrateDBWhenNecessary<CoreDbContext>();
app.MigrateDBWhenNecessary<IdempotentDbContext>();

app.UseRouting();
app.UseErrorSimulatorMiddleware(configuration);
app.UseAuthorization();
app.MapHub<ClientOperationsHub>($"/client");//{configuration.GetSection("SignalRPath")}/client

app.UseWebSockets();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/ws", app.Services.GetRequiredService<CustomWebSocketManager>().HandleWebSocket);
});

app.UseMiddleware<IdempotentRequestsMiddleware>();
app.MapControllers();

BankAccountInitializer.InitializeBankAccount( app.Services, configuration);
app.Run();