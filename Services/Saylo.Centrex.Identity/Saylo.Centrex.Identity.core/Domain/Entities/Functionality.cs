using Saylo.Centrex.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Domain.Entities
{
    public class Functionality : AggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public ScopeFunctionality Scope { get; set; }
        public TypeFunctionality Type { get; set; }
        public ICollection<AdministrationDomain> AdministrationDomains { get; set; } 
        public virtual Module Module { get; set; }
    }

    public enum ScopeFunctionality
    {
        Entreprise,
        ServiceProvider,
        User
    }

    public enum TypeFunctionality
    {
        Read,
        Create,
        update,
        detete
    }
}
