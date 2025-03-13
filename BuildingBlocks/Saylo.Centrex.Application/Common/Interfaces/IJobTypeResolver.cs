namespace Saylo.Centrex.Application.Common.Interfaces;

public interface IJobTypeResolver
{
    Type ResolveJobType(string jobName);
}