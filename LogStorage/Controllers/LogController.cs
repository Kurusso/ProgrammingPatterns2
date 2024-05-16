using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Text;

namespace LogStorage.Controllers
{
    [ApiController]
    [Route("api/log")]
    public class LogController : ControllerBase
    {
        [HttpPost]
        [Consumes("text/plain")]
        public async Task<IActionResult> SaveLogs()
        {

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string logData = await reader.ReadToEndAsync(); 
                Console.WriteLine(logData);
                var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "log.log");

                // Ensure the directory exists.
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

                // Append the log to the file.
                await System.IO.File.AppendAllTextAsync(logFilePath, logData + Environment.NewLine);

                

                // Return a success response.
                return Ok("Log received and saved.");
            }
            
           
        }
    }
}