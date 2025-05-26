using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace atgv.API.Extensions
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _log) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _log.LogError(exception, "Global Exception Message: " + exception.Message);

            var details = new ProblemDetails
            {
                Detail = $"API Error",
                Instance = "ATGV API",
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "API Error",
                Type = "Internal Server Error"
            };

            var response = JsonSerializer.Serialize(details);

            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsync(response, cancellationToken);

            return true;
        }
    }
}
