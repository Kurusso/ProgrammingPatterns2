using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
namespace Common.Middleware
{
    public class ErrorSimulatorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ErrorSimulatorMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var errorConfigurations = _configuration.GetSection("ErrorSettings");

            var random = new Random().Next(100);
            var currentTime = DateTime.Now;
            double errorProbability = errorConfigurations.GetValue<int>("OddMinuteErrorChance"); 

            if (currentTime.Minute % 2 == 0)
            {
                errorProbability = errorConfigurations.GetValue<int>("EvenMinuteErrorChance");
            }

            if (random < errorProbability)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal Server Error");
            }
            else
            {
                await _next(context);
            }
        }
    }
    public static class ErrorSimulatorMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorSimulatorMiddleware(this IApplicationBuilder builder, IConfiguration configuration)
        {
            return builder.UseMiddleware<ErrorSimulatorMiddleware>();
        }
    }
}
