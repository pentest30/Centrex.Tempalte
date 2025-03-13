namespace Saylo.Centrex.Application.Exceptions;

public class JobSchedulerException : Exception
{
    public JobSchedulerException(string message, Exception? innerException = null) 
        : base(message, innerException)
    {
    }
}

public class JobNotFoundException : JobSchedulerException
{
    public JobNotFoundException(string jobName) 
        : base($"Job type '{jobName}' not found.")
    {
    }
}

public class InvalidJobTypeException : JobSchedulerException
{
    public InvalidJobTypeException(string jobName, string reason) 
        : base($"Invalid job type '{jobName}': {reason}")
    {
    }
}
public class JobCreationException : JobSchedulerException
{
    public JobCreationException(string jobName, Exception? innerException = null) 
        : base($"Failed to create instance of job '{jobName}'", innerException)
    {
    }
}

public class JobDelegateCreationException : JobSchedulerException
{
    public JobDelegateCreationException(string jobName, Exception innerException) 
        : base($"Failed to create delegate for job '{jobName}'", innerException)
    {
    }
}

public class JobRegistrationException : JobSchedulerException
{
    public JobRegistrationException(string jobName, Exception innerException) 
        : base($"Failed to register job '{jobName}' with job service", innerException)
    {
    }
}