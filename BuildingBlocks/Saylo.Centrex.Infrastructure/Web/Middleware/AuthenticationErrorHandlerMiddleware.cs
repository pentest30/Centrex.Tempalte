using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Saylo.Centrex.Infrastructure.Web.Middleware;

public class AuthenticationErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    private class ErrorResponse
    {
        public string Message { get; set; }
        public string Details { get; set; }
        public int StatusCode { get; set; }
    }

    public AuthenticationErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (SecurityTokenExpiredException)
        {
            await HandleExceptionAsync(context, "Token has expired. Please login again.", HttpStatusCode.Unauthorized);
        }
        catch (SecurityTokenValidationException)
        {
            await HandleExceptionAsync(context, "Invalid token. Please provide a valid authentication token.", HttpStatusCode.Unauthorized);
        }
        catch (Exception ex) when (IsAuthenticationError(context, ex))
        {
            await HandleExceptionAsync(context, "Authentication failed. Please check your credentials.", HttpStatusCode.Unauthorized);
        }
    }

    private bool IsAuthenticationError(HttpContext context, Exception ex)
    {
        // Check if the error is related to authentication
        return context.Response.StatusCode == 401 || 
               context.Response.StatusCode == 403 ||
               ex.Message.Contains("authentication", StringComparison.OrdinalIgnoreCase) ||
               ex.Message.Contains("unauthorized", StringComparison.OrdinalIgnoreCase);
    }

    private async Task HandleExceptionAsync(HttpContext context, string message, HttpStatusCode statusCode)
    {
        var response = new ErrorResponse
        {
            Message = message,
            StatusCode = (int)statusCode,
            Details = "Please ensure you are logged in and have the necessary permissions."
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}