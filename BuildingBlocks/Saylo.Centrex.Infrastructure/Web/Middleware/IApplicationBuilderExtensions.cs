using Microsoft.AspNetCore.Builder;

namespace Saylo.Centrex.Infrastructure.Web.Middleware
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAuthenticationErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationErrorHandlerMiddleware>();
        }
        public static IApplicationBuilder UseIPFiltering(this IApplicationBuilder app)
        {
            app.UseMiddleware<IPFilteringMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, Dictionary<string, string> headers)
        {
            app.UseMiddleware<SecurityHeadersMiddleware>(headers);
            return app;
        }

        public static IApplicationBuilder UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder app, GlobalExceptionHandlerMiddlewareOptions options = default)
        {
            options ??= new GlobalExceptionHandlerMiddlewareOptions();
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>(options);
            return app;
        }

        public static IApplicationBuilder UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder app, Action<GlobalExceptionHandlerMiddlewareOptions> configureOptions)
        {
            var options = new GlobalExceptionHandlerMiddlewareOptions();
            configureOptions(options);
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>(options);
            return app;
        }

        public static IApplicationBuilder UseLoggingStatusCodeMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<LoggingStatusCodeMiddleware>();
            return app;
        }
    }
}
