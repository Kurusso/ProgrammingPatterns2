using client_bank_backend.Hubs;
using client_bank_backend.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddSingleton<IHostedService, AccountHubService>();

services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            corsPolicyBuilder
                .WithOrigins("http://localhost:3000")
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


app.UseAuthorization();
app.UseCors();
app.MapHub<BffAccountHub>("/AccountHub");
app.MapControllers();

app.Run();