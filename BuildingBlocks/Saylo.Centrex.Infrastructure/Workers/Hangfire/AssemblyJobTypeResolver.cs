using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Application.Exceptions;

namespace Saylo.Centrex.Infrastructure.Workers.Hangfire;

public class AssemblyJobTypeResolver : IJobTypeResolver
{
    public Type ResolveJobType(string jobName)
    {
        ArgumentException.ThrowIfNullOrEmpty(jobName);

        return AppDomain.CurrentDomain.GetAssemblies()
                   .SelectMany(assembly => assembly.GetTypes())
                   .FirstOrDefault(t => t.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase))
               ?? throw new JobNotFoundException(jobName);
    }
}