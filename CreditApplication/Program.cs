using CreditApplication.Models;
using CreditApplication.Quartz;
using CreditApplication.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
services.AddSwaggerGen();
services.AddDbContext<CreditDbContext>(options =>  options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection")
    )
);
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

app.Run();
