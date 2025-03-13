using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

namespace Saylo.Centrex.Identity.Core.Application.Models;

public class RegisterRequest
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public TypeUser TypeUser { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
