using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Diagnostics;
using System.Text.Json;


namespace Common.Middlewares
{
    public class TracingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TracingMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private const int ReadChunkBufferLength = 4096;

        public TracingMiddleware(RequestDelegate next, ILogger<TracingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();            
            if (!Guid.TryParse(context.Request.Headers["TraceId"], out var traceIdentifier))
            {
                traceIdentifier = Guid.NewGuid();
                context.Request.Headers.Add("TraceId", traceIdentifier.ToString());
            }

            var endpoint = context.GetEndpoint();
            var stopwatch = new Stopwatch();
            var model = new RequestTracingModel
            {
                DateTimeUTC = DateTime.UtcNow,
                TraceId = traceIdentifier,
                EndpointName = endpoint?.DisplayName ?? "Unknown",
                Method = context.Request.Method,
                Metadata = endpoint?.Metadata?.ToString() ?? string.Empty
            };

            LogRequest(context.Request, model, traceIdentifier);            

            stopwatch.Start();
            var originalBody = context.Response.Body;
            using (var responseStream = _recyclableMemoryStreamManager.GetStream())
            {
                context.Response.Body = responseStream;

                await _next.Invoke(context);

                stopwatch.Stop();
                model.StatusCode = (System.Net.HttpStatusCode)context.Response.StatusCode;

                responseStream.Seek(0, SeekOrigin.Begin);
                _logger.LogInformation("End processing HTTP request after {@ElapsedMilliseconds}ms - {@StatusCode}"+Environment.NewLine
                                       + "TraceId: {@TraceId}"+Environment.NewLine
                                       + "Schema: {@Schema}" + Environment.NewLine
                                       + "Host: {@Host}" + Environment.NewLine
                                       + "Path: {@Path}" + Environment.NewLine
                                       + "QueryString: {@QueryString} " + Environment.NewLine
                                       + "Response Body: {@Body}", 
                                       stopwatch.ElapsedMilliseconds, 
                                       context.Response.StatusCode, 
                                       traceIdentifier,
                                       context.Request.Scheme,
                                       context.Request.Host,
                                       context.Request.Path.Value,
                                       context.Request.QueryString.Value,
                                       TryDeserializeJsonContent(ReadStreamInChunks(responseStream)));


                responseStream.Seek(0, SeekOrigin.Begin);
                await responseStream.CopyToAsync(originalBody);
            }

            context.Response.Body = originalBody;
        }

        public void LogRequest(HttpRequest request, RequestTracingModel model, Guid traceId)
        {
            using (var requestStream = _recyclableMemoryStreamManager.GetStream())
            {
                request.Body.CopyTo(requestStream);
                request.Body.Seek(0, SeekOrigin.Begin);
                _logger.LogInformation("HTTP Request information:" + Environment.NewLine
                                       + "TraceId: {@TraceId}" + Environment.NewLine
                                       + "Schema: {@Schema}" + Environment.NewLine
                                       + "Host: {@Host}" + Environment.NewLine
                                       + "Path: {@Path}" + Environment.NewLine
                                       + "QueryString: {@QueryString} " + Environment.NewLine
                                       + "Response Body: {@Body}",
                                       traceId,
                                       request.Scheme,
                                       request.Host,
                                       request.Path.Value,
                                       request.QueryString.Value,
                                       TryDeserializeJsonContent(ReadStreamInChunks(requestStream)));
            }
        }

        private static object TryDeserializeJsonContent(string str) {
            try
            {
                var obj = JsonConvert.DeserializeObject<dynamic>(str);
                return obj ?? str;
            }
            catch (Exception)
            {
                return str;
            }
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string result;
            using (var textWriter = new StringWriter())
            using (var reader = new StreamReader(stream, leaveOpen: true))
            {
                var readChunk = new char[ReadChunkBufferLength];
                int readChunkLength;
                do
                {
                    readChunkLength = reader.ReadBlock(readChunk, 0, ReadChunkBufferLength);
                    textWriter.Write(readChunk, 0, readChunkLength);
                } while (readChunkLength > 0);

                result = textWriter.ToString();
            }
            
            return result;
        }
    }
}
