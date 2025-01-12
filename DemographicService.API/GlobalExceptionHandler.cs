using Demographic.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DemographicService.API
{
    internal class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred while processing the request");
            Activity? activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

            var problemDetails = exception switch
            {
                CustomValidationException validationException => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "ValidationFailure",
                    Title = "Validation error",
                    Detail = "",
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                    Extensions = new Dictionary<string, object?>
                    {
                        ["errors"] = validationException.Errors,
                        ["traceId"] = activity?.Id,
                        ["requestId"] = httpContext.TraceIdentifier
                    }
                },
                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = exception.GetType().Name,
                    Title = "An error occurred while processing your request",
                    Detail = exception.Message,
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                    Extensions = new Dictionary<string, object?>
                    {
                        ["traceId"] = activity?.Id,
                        ["requestId"] = httpContext.TraceIdentifier
                    }
                }
            };

            httpContext.Response.StatusCode = problemDetails.Status ?? 500;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
