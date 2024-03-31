using CoreApplication.BackgroundJobs;
using CoreApplication.Configurations;
using CoreApplication.Hubs;
using CoreApplication.Initialization;
using CoreApplication.Models;
using CoreApplication.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddScoped<IAccountService, AccountService>();
services.AddScoped<IMoneyOperationsService, MoneyOperationsService>();
services.AddScoped<IUserService, UserService>();
services.AddSwaggerGen();
services.AddSignalR();
services.AddHostedService<OperationsListener>();
services.AddDbContext<CoreDbContext>(options =>
    options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection")
    )
);

var notificationSettings = builder.Configuration.GetSection("RabbitMqConfigurations").Get<RabbitMqConfigurations>();
builder.Services.Configure<RabbitMqConfigurations>(builder.Configuration.GetSection("RabbitMqConfigurations"));
builder.RegisterBackgroundJobs(configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.MapHub<ClientOperationsHub>("/api/client");
BankAccountInitializer.InitializeBankAccount( app.Services, configuration);
app.Run();