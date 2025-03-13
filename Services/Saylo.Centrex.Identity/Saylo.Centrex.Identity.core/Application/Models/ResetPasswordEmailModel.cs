namespace Saylo.Centrex.Identity.Core.Application.Models;

public class ResetPasswordEmailModel
{
    public string UserName { get; set; }
    public string ResetPasswordUrl { get; set; }
    public string CompanyName { get; set; }
}