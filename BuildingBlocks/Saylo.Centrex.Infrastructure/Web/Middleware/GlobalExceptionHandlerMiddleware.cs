using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saylo.Centrex.Application.Exceptions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Saylo.Centrex.Infrastructure.Web.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly GlobalExceptionHandlerMiddlewareOptions _options;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            GlobalExceptionHandlerMiddlewareOptions options)
        {
            _next = next;
            _logger = logger;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                if (ex is ValidationException validationException)
                {
                    var errors = validationException.Errors
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    var rs = new
                    {
                        message = "Validation error", 
                        errors
                    };

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(rs));
                    return;
                }

                if (ex is NotFoundException)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                else
                {
                    _logger.LogError(ex, "[{0}-{1}]", DateTime.UtcNow.Ticks, Thread.CurrentThread.ManagedThreadId);
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
                var result = JsonSerializer.Serialize(new { message = GetErrorMessage(ex) });
                await response.WriteAsync(result);
            }

        }

        private string GetErrorMessage(Exception ex)
        {
           

            return _options.DetailLevel switch
            {
                GlobalExceptionDetailLevel.None => "An internal exception has occurred.",
                GlobalExceptionDetailLevel.Message => ex.Message,
                GlobalExceptionDetailLevel.StackTrace => ex.StackTrace,
                GlobalExceptionDetailLevel.ToString => ex.ToString(),
                _ => "An internal exception has occurred.",
            };
        }
    }

    public class GlobalExceptionHandlerMiddlewareOptions
    {
        public GlobalExceptionDetailLevel DetailLevel { get; set; }
    }

    public enum GlobalExceptionDetailLevel
    {
        None,
        Message,
        StackTrace,
        ToString,
        Throw,
    }
}
