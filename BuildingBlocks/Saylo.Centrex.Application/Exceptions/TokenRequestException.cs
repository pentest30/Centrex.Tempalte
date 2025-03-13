namespace Saylo.Centrex.Application.Exceptions;

public class TokenRequestException : Exception
{
    public TokenRequestException(string message) : base(message) { }
}