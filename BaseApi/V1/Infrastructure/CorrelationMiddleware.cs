using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BaseApi.V1.Infrastructure
{
    public class CorrelationMiddleware
    {
        private readonly RequestDelegate _next;
        private string _correlationid = "x-correlationId";

        public CorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers[_correlationid].Count == 0)
            {
                context.Request.Headers[_correlationid] = Guid.NewGuid().ToString();
            }

            if (_next != null)
                await _next(context).ConfigureAwait(false);
        }
    }

    public static class CorrelationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelation(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationMiddleware>();
        }
    }
}
