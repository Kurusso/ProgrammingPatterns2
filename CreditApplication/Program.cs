using Common.Helpers;
using Common.Models;
using Common.Services;
using CoreApplication.BackgroundJobs;
using CreditApplication.Models;
using CreditApplication.Quartz;
using CreditApplication.Services;
using Microsoft.EntityFrameworkCore;
using Common.Middleware;
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
services.AddSwaggerGen(c => {
    c.OperationFilter<ExposeIdempotentIdSwaggerFilter>();
});
services.AddDbContext<CreditDbContext>(options =>  options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection")
    )
);
builder.AddIdempotenceDB("IdempotenceDbConnection");
builder.AddIdempotentAutoRetryHttpClient();
// services.AddMvc().AddJsonOptions(options =>
// {
//     options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
//     options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
// });
services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}
// app.UseHttpsRedirection();

app.MigrateDBWhenNecessary<IdempotentDbContext>();
app.MigrateDBWhenNecessary<CreditDbContext>();
app.UseErrorSimulatorMiddleware(configuration);
app.UseAuthorization();



app.MapHealthChecks("/health");

app.UseMiddleware<IdempotentRequestsMiddleware>();
app.MapControllers();

app.Run();
