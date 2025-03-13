using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using OpenIddict.Server;
using Saylo.Centrex.Identity.Core.Infrastructure.Configuration;
using Saylo.Centrex.Identity.Core.Infrastructure.Services;
using OpenIddict.Validation.AspNetCore;
using Saylo.Centrex.Identity.Core;
using Saylo.Centrex.Identity.Core.Application.Commands.Identity;
using Saylo.Centrex.Infrastructure.Logging;
using Saylo.Centrex.Infrastructure.MultiTenancy.Configuration.Extensions;
using Saylo.Centrex.Infrastructure.Services;
using Saylo.Centrex.Infrastructure.Swagger;
using Saylo.Centrex.Infrastructure.Web.Middleware;
using Saylo.Centrex.Infrastructure.Workers.Hangfire;

var builder = WebApplication.CreateBuilder(args);


var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();

var appSettings = new AppSettings();
configuration.Bind(appSettings);
builder.Services.AddSingleton(appSettings);
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRoleCommand>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto|
                               ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddAutoMapper(typeof(CreateUserRoleCommand));
builder.Services.AddIdentityCore(builder.Configuration);
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.WebHost.UseFileLogger(builder.Configuration);
builder.Logging.AddConsole();
var app = builder.Build();
app.Use((context, next) => 
{
    if (context.Request.Headers.ContainsKey("X-Forwarded-Proto"))
    {
        context.Request.Scheme = context.Request.Headers["X-Forwarded-Proto"].ToString().ToLower();
    }
    return next();
});
app.UseRequestLocalization();
app.UseCors("AllowAll");
app.UseGlobalExceptionHandlerMiddleware(options => options.DetailLevel = GlobalExceptionDetailLevel.Message);
app.UseLoggingStatusCodeMiddleware();
app.UseHangfireDashboard(builder.Configuration);

app.UseSwaggerConfiguration(builder.Configuration);
var jobScheduler = app.Services.GetRequiredService<HangfireJobScheduler>();
jobScheduler.CreateRecurrentJobs();
app.UseRouting();
app.UseCors();

// Configure the HTTP request pipeline. it should be after UseRouting
app.UseAuthentication();
app.UseAuthorization();
app.UseMultiTenancy();
app.UseAuthenticationErrorHandler();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var seedService = scope.ServiceProvider.GetRequiredService<SeedDummyDataService>();
    await seedService.SeedAsync();
}
app.Run();