using Common.Helpers;
using Common.Models;
using Common.Services;
using Microsoft.EntityFrameworkCore;
using UserService.Helpers;
using UserService.Models;
using UserService.Services;
using Common.Middleware;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => {
    c.OperationFilter<ExposeIdempotentIdSwaggerFilter>();
});
builder.Services.AddRazorPages();

builder.Services.AddScoped<AuthService, AuthService>();
builder.Services.AddScoped<UsersService, UsersService>();
builder.Services.AddScoped<HttpClient>(options =>
{
    var messageHandler = new IdempotentAutoRetryHttpMessageHandler();
    return new HttpClient(messageHandler);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            corsPolicyBuilder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
// builder.Services.AddScoped<ClientService, ClientService>();

builder.AddIdentity();

builder.Services.AddDbContext<MainDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"));
    options.UseOpenIddict();
});
builder.AddIdempotenceDB("IdempotenceDbConnection");
builder.AddOpenIddict();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MigrateDBWhenNecessary<MainDbContext>();
app.MigrateDBWhenNecessary<IdempotentDbContext>();
app.AddOauthClients();
app.InitRoles();

app.UseErrorSimulatorMiddleware(configuration);
app.UseMiddleware<MyMiddleware>();
app.UseStaticFiles();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<IdempotentRequestsMiddleware>();
app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();


app.Run();
