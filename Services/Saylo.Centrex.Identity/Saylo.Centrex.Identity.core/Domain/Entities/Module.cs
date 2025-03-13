using Saylo.Centrex.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Domain.Entities;

public class Module : AggregateRoot<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ICollection<Functionality> Functionalities { get; set; }
}