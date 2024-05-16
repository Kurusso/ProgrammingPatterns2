using Common.CurrencyApi;
using Common.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quartz;
using Serilog.Context;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Common.Jobs
{
    public class LogPublishingJob : IJob
    {
        private readonly string _publishLogs;
        private readonly HttpClient _httpClient;
        private readonly string _token;
        private ConcurrentQueue<string> _logQueue;
        private Thread _processingThread;
        private CancellationTokenSource _cancellationTokenSource;
        public LogPublishingJob(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            var logSection = configuration.GetSection("LogCollection");
            _publishLogs = logSection["Publish"]!;
            _token = logSection["Token"]!;
            _httpClient = httpClientFactory.CreateClient($"{nameof(LogPublishingJob)}HttpClient");
            _logQueue = new();
            _cancellationTokenSource = new CancellationTokenSource();
            _processingThread = new Thread(async () => await ProcessEntries(_cancellationTokenSource.Token));

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("X-API-TOKEN", _token);
        }
        public async Task ProcessEntries(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_logQueue.IsEmpty)
                {
                    Thread.Sleep(1000);
                }

                try
                {
                    if (_logQueue.TryDequeue(out var file))
                    {
                        var body = await File.ReadAllTextAsync(file);
                        var jsonBody = JsonConvert.SerializeObject(body); // Convert the body to JSON
                        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json"); // Change the content type to application/json

                        var response = await _httpClient.PostAsync(_publishLogs, content);
                        response.EnsureSuccessStatusCode();
                        File.Delete(file);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var files = Directory.GetFiles("./logs", "log*.txt").SkipLast(1).Where(x => !_logQueue.Contains(x)).OrderBy(x => x);
            foreach (var file in files)
            {
                _logQueue.Enqueue(file);
            }

            if (!_processingThread.IsAlive)
            {
                _processingThread.Start();
            }
        }
    }
}
