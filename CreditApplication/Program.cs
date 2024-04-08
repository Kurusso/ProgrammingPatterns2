using CoreApplication.BackgroundJobs;
using CreditApplication.Models;
using CreditApplication.Quartz;
using CreditApplication.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

builder.RegisterBackgroundJobs(configuration);
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddScoped<ICreditService, CreditService>();
services.AddScoped<ICreditRateService, CreditRateService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<ICreditPenaltyService, CreditPenaltyService>();
services.AddScoped<ICreditScoreService, CreditScoreService>();
services.AddScoped<IRabbitMqService, RabbitMQIntegrationService>();
services.AddHostedService<RabbitMQFeedbackListener>();
services.AddSwaggerGen();
services.AddDbContext<CreditDbContext>(options =>  options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection")
    )
);
services.AddMvc().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
services.AddHealthChecks();

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

app.MapHealthChecks("/health");

app.Run();
