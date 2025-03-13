namespace Saylo.Centrex.Identity.Core.Application.Models;

public class IdentityResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; } = new();

    private IdentityResponse(bool succeeded, string message = null, IEnumerable<string> errors = null)
    {
        Succeeded = succeeded;
        Message = message;
        Errors = errors?.ToList() ?? new List<string>();
    }

    public static IdentityResponse Success(string message = null) =>
        new(true, message);

    public static IdentityResponse Failure(string error) =>
        new(false, errors: new[] { error });

    public static IdentityResponse Failure(IEnumerable<string> errors) =>
        new(false, errors: errors);
}