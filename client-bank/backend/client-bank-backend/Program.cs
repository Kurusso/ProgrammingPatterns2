using client_bank_backend.Hubs;
using client_bank_backend.Services;
using client_bank_backend.Services.RabbitMqServices;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddSingleton<IHostedService, AccountHubService>();
services.AddScoped<IRabbitMqService, RabbitMQIntegrationService>();
    //services.AddHostedService< RabbitMQFeedbackListener>();


builder.Services.AddHttpClient();
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

app.MapHub<BffAccountHub>("/AccountHub");
app.MapControllers();

app.Run();