using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace AntaresClientApi.Middleware
{
    public class LogAllMiddleware
    {
        private readonly RequestDelegate _next;
        const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} {StatusCode} finished in {Elapsed:0.0000} ms";

        public LogAllMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                sw.Stop();
                Log.Error(ex, ex.Message);
                throw;
            }

            sw.Stop();

            if (context.Request.Path == "/api/isalive")
                return;

            Log.Information(MessageTemplate,  context.Request.Method, $"{context.Request.Path}{context.Request.QueryString}", context.Response.StatusCode, sw.Elapsed.TotalMilliseconds);
        }
    }
}
