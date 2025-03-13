namespace Saylo.Centrex.Domain.Entities;

public abstract class TenantInfoBase : AggregateRoot<Guid>
{
    protected TenantInfoBase()
    {
        Id = Guid.NewGuid();
    }

    protected TenantInfoBase(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));
        Name = name;
        Id = Guid.NewGuid();
    }
    public string Name { get; set; } = null!;
    public string? ConnectionString { get; set; }
    public string? Domain { get; set; } 
    public bool IsActive { get; set; }
    public string? Email { get; set; }
    public string? Administrator { get; set; }
}
