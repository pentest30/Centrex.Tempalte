namespace Saylo.Centrex.Identity.Core.Infrastructure.Configuration
{

        public interface IRequestContext
        {         
            public Guid?  TenantId { get; set; }
            public List<Guid?> TenantIds { get; set; } 
            public bool isAnonyme { get; set; }
            public bool isContextEtableshed { get; set; }
        }
    

   
}
