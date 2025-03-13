namespace Saylo.Centrex.Application.Common.Interfaces;

public interface IJobScheduler
{
    void ScheduleJob(string jobName, string cronExpression);
}