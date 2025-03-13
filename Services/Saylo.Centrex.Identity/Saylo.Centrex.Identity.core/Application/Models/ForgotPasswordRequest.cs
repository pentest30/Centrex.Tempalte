namespace Saylo.Centrex.Identity.Core.Application.Models;

public class ForgotPasswordRequest
{
     public string Email { get; set; }
}
public class ResetPasswordRequest
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }
}