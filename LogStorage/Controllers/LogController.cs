using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace LogStorage.Controllers
{
    [ApiController]
    [Route("api/log")]
    public class LogController : ControllerBase
    {[HttpPost]
        public async Task<IActionResult> SaveLogs([FromBody] string logData)
        {
            // Specify the relative path to the log file.
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "log.log");

            // Ensure the directory exists.
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

            // Append the log to the file.
            await System.IO.File.AppendAllTextAsync(logFilePath, logData + Environment.NewLine);

            Console.WriteLine(logData);

            // Return a success response.
            return Ok("Log received and saved.");
        }
    }
}