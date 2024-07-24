using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BL.Middleware
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            context.Response.OnStarting(() =>
            {
                stopwatch.Stop();
                var responseTimeForCompleteRequest = stopwatch.ElapsedMilliseconds;
                _logger.LogInformation($"Request [{context.Request.Method}] at [{context.Request.Path}] took {responseTimeForCompleteRequest} ms");
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}