using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dfe.FE.Interventions.Api.Infrastructure.Middleware
{
    public class LogCorrelationMiddleware
    {
        private readonly RequestDelegate _next;

        public LogCorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ILogger<LogCorrelationMiddleware> logger)
        {
            var requestId = Guid.NewGuid().ToString();
            var clientRequestIdHeaderName = httpContext.Request.Headers.Keys
                .SingleOrDefault(x => x.Equals("Client-Request-ID", StringComparison.InvariantCultureIgnoreCase));
            var clientRequestId = clientRequestIdHeaderName != null ? httpContext.Request.Headers[clientRequestIdHeaderName].FirstOrDefault() : null;

            using (logger.BeginScope(new Dictionary<string, object>
            {
                {"RequestId", requestId},
                {"ClientRequestId", clientRequestId},
            }))
            {
                httpContext.Response.Headers.Add("Server-Request-ID", requestId);
                if (!string.IsNullOrEmpty(clientRequestId))
                {
                    httpContext.Response.Headers.Add("Client-Request-ID", clientRequestId);
                }
                
                await _next(httpContext);
            }
        }
    }
}