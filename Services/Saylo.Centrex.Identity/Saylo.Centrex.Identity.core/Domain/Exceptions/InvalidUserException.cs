namespace Saylo.Centrex.Identity.Core.Domain.Exceptions
{
    public class InvalidUserException : DomainException
    {
        public InvalidUserException(string message) : base(message) { }
    }

}
