using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Application.Exceptions;

namespace Saylo.Centrex.Infrastructure.Workers.Hangfire;

public class JobScheduler : IJobScheduler
{
    private readonly IJobService _jobService;
    private readonly IJobTypeResolver _jobTypeResolver;
    private readonly ILogger<JobScheduler> _logger;

    public JobScheduler(
        IJobService jobService,
        IJobTypeResolver jobTypeResolver,
        ILogger<JobScheduler> logger)
    {
        _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
        _jobTypeResolver = jobTypeResolver ?? throw new ArgumentNullException(nameof(jobTypeResolver));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void ScheduleJob(string jobName, string cronExpression)
    {
        try
        {
            ArgumentException.ThrowIfNullOrEmpty(jobName);
            ArgumentException.ThrowIfNullOrEmpty(cronExpression);

            var jobType = _jobTypeResolver.ResolveJobType(jobName);
            ValidateJobType(jobType);
            AddOrUpdateJob(jobName, jobType, cronExpression);
            
            _logger.LogInformation("Successfully scheduled job {JobName} with cron expression {CronExpression}", 
                jobName, cronExpression);
        }
        catch (Exception ex) when (ex is not JobSchedulerException)
        {
            var error = $"Failed to schedule job {jobName}";
            _logger.LogError(ex, error);
            throw new JobSchedulerException(error, ex);
        }
    }

    private void ValidateJobType(Type jobType)
    {
        var processAsyncMethod = jobType.GetMethod("ProcessAsync");
        if (processAsyncMethod == null)
        {
            throw new InvalidJobTypeException(jobType.Name, "Missing ProcessAsync method");
        }

        var returnType = processAsyncMethod.ReturnType;
        if (!typeof(Task).IsAssignableFrom(returnType))
        {
            throw new InvalidJobTypeException(jobType.Name, 
                "ProcessAsync method must return Task or Task<T>");
        }

        var parameters = processAsyncMethod.GetParameters();
        if (parameters.Length != 1 || parameters[0].ParameterType != typeof(CancellationToken))
        {
            throw new InvalidJobTypeException(jobType.Name, 
                "ProcessAsync method must accept a single CancellationToken parameter");
        }
    }
    private void AddOrUpdateJob(
        string jobName, 
        Type jobType, 
        string cronExpression)
    {
        var method = typeof(IJobService)
            .GetMethod(nameof(IJobService.AddOrUpdate))!
            .MakeGenericMethod(jobType);

        var jobParameter = Expression.Parameter(jobType, "job");
        var processAsyncMethod = jobType.GetMethod("ProcessAsync");

        if (processAsyncMethod == null)
        {
            throw new InvalidJobTypeException(jobType.Name, "Missing ProcessAsync method");
        }

        var cancellationToken = Expression.Constant(CancellationToken.None);
        var methodCall = Expression.Call(jobParameter, processAsyncMethod, cancellationToken); 
        var lambda = Expression.Lambda(methodCall, jobParameter);
        var cronExpressionFunc = new Func<string>(() => cronExpression);

        var parameters = new object[]
        {
            jobName,
            lambda,
            cronExpressionFunc,
            TimeZoneInfo.Local,
            "default"
        };

        try
        {
            method.Invoke(_jobService, parameters);
        }
        catch (Exception ex)
        {
            throw new JobRegistrationException(jobName, ex);
        }
    }
}