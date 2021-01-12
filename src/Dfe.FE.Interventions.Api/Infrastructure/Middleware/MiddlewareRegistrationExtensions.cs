using Microsoft.AspNetCore.Builder;

namespace Dfe.FE.Interventions.Api.Infrastructure.Middleware
{
    public static class MiddlewareRegistrationExtensions
    {
        public static IApplicationBuilder UseLoggingCorrelation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogCorrelationMiddleware>();
        }
    }
}