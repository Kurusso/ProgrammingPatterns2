using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using UserService.Helpers;
using UserService.Models;
using UserService.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();

builder.Services.AddScoped<AuthService, AuthService>();
builder.Services.AddScoped<UsersService, UsersService>();
// builder.Services.AddScoped<ClientService, ClientService>();

builder.AddIdentity();
builder.AddDB<MainDbContext>("DbConnection");
builder.AddOpenIddict();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MigrateDBWhenNecessary<MainDbContext>();
app.AddOauthClients();
app.InitRoles();


app.UseMiddleware<MyMiddleware>();
app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();


app.Run();
