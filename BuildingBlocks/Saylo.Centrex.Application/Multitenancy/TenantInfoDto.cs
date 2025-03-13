namespace Saylo.Centrex.Application.Multitenancy;

public class TenantInfoDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? ParentId { get; set; }
}