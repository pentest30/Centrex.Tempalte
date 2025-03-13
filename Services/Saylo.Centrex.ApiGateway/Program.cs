using Saylo.Centrex.ApiGateway;
using Saylo.Centrex.Infrastructure.MultiTenancy.Middleware;
using Saylo.Centrex.Infrastructure.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGatewayConfiguration(builder.Configuration);

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowAll");
// Global error handling
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseAuthenticationErrorHandler();
app.UseMiddleware<TenantAuthorizationMiddleware>();

app.MapHealthChecks("/health");

app.MapReverseProxy();
app.UseWebSockets();
app.Run();
