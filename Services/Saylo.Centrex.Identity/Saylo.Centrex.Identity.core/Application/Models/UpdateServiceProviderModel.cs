using Saylo.Centrex.Application.Common.Automapper;
using Saylo.Centrex.Identity.Core.Application.Commands.AdministrationDomains.ServiceProviders;

namespace Saylo.Centrex.Identity.Core.Application.Models;

public class UpdateServiceProviderModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string ServiceType { get; set; } 
    public string ContactEmail { get; set; }
}