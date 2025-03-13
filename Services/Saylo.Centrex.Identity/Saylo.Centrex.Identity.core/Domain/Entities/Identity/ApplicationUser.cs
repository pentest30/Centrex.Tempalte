using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Events;
using Saylo.Centrex.Identity.Core.Domain.DomainEvents.Identity;

namespace Saylo.Centrex.Identity.Core.Domain.Entities.Identity;
public  class ApplicationUser : IdentityUser<Guid>, IAggregateRoot, ITenantEntity
{
    [NotMapped]
    private readonly List<IDomainEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public ApplicationUser()
    {
    }
    
    public ApplicationUser(string userName, string? email, string password, TypeUser typeUser, string ? correlationId) : base(userName)
    {
        Id = Guid.NewGuid();
        Email = email;
        NormalizedEmail = email.ToUpper();
        NormalizedUserName = userName.ToUpper();
        Password = password;
        TypeUser = typeUser;
        IsActive = true;
        UserRoles = new List<AdminUserRole>();
        AddDomainEvent(new UserCreatedEvent(this, correlationId));
    }
    public string Password { get; }
    public TypeUser TypeUser { get; set; }
    public ICollection<AdminUserRole> UserRoles { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; }
    public AdministrationDomain Tenant { get; set; }
    public Guid? TenantId { get; set; }

    public bool IsAdmin => TypeUser == TypeUser.Admin;
    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    // Clear domain events
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    
}
