using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Infrastructure.Workers.Hangfire;

namespace Saylo.Centrex.Infrastructure.Services;

public class HangfireJobScheduler
{
    private readonly HangfireSettings _jobConfigs;
    private readonly IJobScheduler _jobScheduler;
    public HangfireJobScheduler(HangfireSettings jobConfigs,
        IJobScheduler jobScheduler)
    {
        _jobScheduler = jobScheduler;
        _jobConfigs = jobConfigs;
    }
    public void CreateRecurrentJobs()
    {
        if (_jobConfigs.HangfireJobs == null) return;
        
        foreach (var jobConfig in _jobConfigs.HangfireJobs) 
        {
            _jobScheduler.ScheduleJob(jobConfig.JobName, jobConfig.CronExpression);
        }
    }
}
