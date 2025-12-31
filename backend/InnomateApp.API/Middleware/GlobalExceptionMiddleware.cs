using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace InnomateApp.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unhandled exception occurred during request {RequestId}", context.TraceIdentifier);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = context.Response;
            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = context.TraceIdentifier }
            };

            switch (exception)
            {
                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                    problemDetails.Title = "Unauthorized";
                    problemDetails.Detail = "Access denied.";
                    break;

                case KeyNotFoundException:
                case InnomateApp.Domain.Common.EntityNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    problemDetails.Status = (int)HttpStatusCode.NotFound;
                    problemDetails.Title = "Not Found";
                    problemDetails.Detail = exception.Message;
                    break;

                case ArgumentException:
                case InvalidOperationException:
                case InnomateApp.Domain.Common.BusinessRuleViolationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Title = "Bad Request";
                    problemDetails.Detail = exception.Message;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Title = "Internal Server Error";
                    problemDetails.Detail = "An unexpected error occurred. Please contact support.";
                    break;
            }

            var json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }
}
