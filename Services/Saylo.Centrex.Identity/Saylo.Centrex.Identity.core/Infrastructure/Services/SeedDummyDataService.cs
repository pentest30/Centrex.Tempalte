using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Identity.Core.Domain.Entities;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;
using Saylo.Centrex.Identity.Core.Infrastructure.Persistence;

namespace Saylo.Centrex.Identity.Core.Infrastructure.Services;

public class SeedDummyDataService
{
    private readonly SayloIdentityDbContext _dbContext;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly ITenantCacheStore _tenantCacheStore;
    private readonly IMapper _mapper;

    public SeedDummyDataService(SayloIdentityDbContext dbContext, 
        IOpenIddictApplicationManager applicationManager, 
        ITenantCacheStore tenantCacheStore,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _applicationManager = applicationManager;
        _tenantCacheStore = tenantCacheStore;
        _mapper = mapper;
    }

    public async Task SeedAsync()
    {
        // Seed Admin User
        await SeedAdminUserAsync();

        // Seed OpenIddict Application
        await SeedOpenIddictApplicationAsync();
        // Seed Tenant Cache
        await SeedTenantCacheAsync();
    }

    private async Task SeedTenantCacheAsync()
    {
        var tenants = await _dbContext.Set<AdministrationDomain>()
            .IgnoreQueryFilters()
            .ToListAsync();
        foreach (var administrationDomain in tenants)
        {
            var tenant = await _tenantCacheStore.GetTenantAsync(administrationDomain.Id.ToString());

            if (tenant == null)
            {
                await _tenantCacheStore.SetTenantAsync(administrationDomain.Id.ToString(), _mapper.Map<TenantInfoDto>(administrationDomain));
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var adminRoleId = Guid.Parse("26b0af42-3f0e-4a11-b28f-85248784a0f0");
        var adminId = Guid.Parse("26b0af42-3f0e-4a11-b28f-85248784a0f2");
        var roles = new List<AdminRole>
        {
            new()
            {
                Id = Guid.Parse("26b0af42-3f0e-4a11-b28f-85248784a0f0"), // Unique identifier for the Admin role
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new()
            {
                Id = Guid.Parse("26b0af42-3f0e-4a11-b28f-85248784a0f1"), // Unique identifier for the User role
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        };
        foreach (var adminRole in roles)
        {
            if (!await _dbContext.Roles.AnyAsync(u => u.Name == adminRole.Name))
               await _dbContext.Roles.AddAsync(adminRole);
        }
        if (!await _dbContext.Users.AnyAsync(u => u.UserName == "admin"))
        {
            var adminUser = new ApplicationUser
            {
                Id = Guid.Parse("26b0af42-3f0e-4a11-b28f-85248784a0f2"),
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@netcom.fr",
                NormalizedEmail = "ADMIN@NETCOM.FR",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd$@00198763.."),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                TenantId = Guid.Parse("BDCB855F-A6A0-444F-997E-0004603D6C93"),
                TypeUser = TypeUser.Admin
            };

            await _dbContext.Users.AddAsync(adminUser); 
        }

        foreach (var chatUser in ChatUsers())
        {
            if (!await _dbContext.Users.AnyAsync(u => u.Id == chatUser.Id))
            {
                await _dbContext.Users.AddAsync(chatUser);
            }                
        }

        if (!await _dbContext.UserRoles.AnyAsync(x => x.RoleId == adminRoleId && x.UserId == adminId))
            await _dbContext.UserRoles.AddAsync(new AdminUserRole
            {
                RoleId = adminRoleId,
                UserId = adminId
            });
        await _dbContext.SaveChangesAsync();

    }

    private async Task SeedOpenIddictApplicationAsync()
    {
        if (await _applicationManager.FindByClientIdAsync("api-client") == null)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "api-client",
                ClientSecret = "api-secret",
                DisplayName = "Swagger UI",
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Introspection,  // Add this
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.Password,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,  // Add this
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                }
            };
            await _applicationManager.CreateAsync(descriptor);
        }
    }

    private List<ApplicationUser> ChatUsers()
    {
        return new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.Parse("9395ae6d-18af-4519-8840-f04bf510318e"),
                UserName = "alice.johnson",
                NormalizedUserName = "ALICE.JOHNSON",
                Email = "alice@example.com",
                NormalizedEmail = "ALICE@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd$@00198763.."),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                FirstName = "Alice",
                LastName = "Johnson"
            },
            new ApplicationUser
            {
                Id = Guid.Parse("d67f76ea-c95b-4492-889e-b9c3ff6a88e1"),
                UserName = "andy.johnson",
                NormalizedUserName = "ANDY.JOHNSON",
                Email = "andy@example.com",
                NormalizedEmail = "ANDY@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd$@00198763.."),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                FirstName = "Andy",
                LastName = "Johnson"
            },
            new ApplicationUser
            {
                Id = Guid.Parse("eef8ccb1-347a-4c34-a673-74506bd3df7a"),
                UserName = "bob.smith",
                NormalizedUserName = "BOB.SMITH",
                Email = "bob@example.com",
                NormalizedEmail = "BOB@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd$@00198763.."),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                FirstName = "Bob",
                LastName = "Smith"
            },
            new ApplicationUser
            {
                Id = Guid.Parse("fcd3fd6d-a57a-4725-9d0b-c4d8baaa9eb6"),
                UserName = "carla.jenkins",
                NormalizedUserName = "CARLA.JENKINS",
                Email = "carla@example.com",
                NormalizedEmail = "CARLA@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd$@00198763.."),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                FirstName = "Carla",
                LastName = "Jenkins"
            },
            new ApplicationUser
            {
                Id = Guid.Parse("e79fd945-e579-4588-b736-97005d436ef5"),
                UserName = "david.brown",
                NormalizedUserName = "DAVID.BROWN",
                Email = "david@example.com",
                NormalizedEmail = "DAVID@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd$@00198763.."),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                FirstName = "David",
                LastName = "Brown"
            },
            new ApplicationUser
            {
                Id = Guid.Parse("aa938837-cb82-4385-ae10-1ccb23f14fc3"),
                UserName = "eva.green",
                NormalizedUserName = "EVA.GREEN",
                Email = "eva@example.com",
                NormalizedEmail = "EVA@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd$@00198763.."),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                FirstName = "Eva",
                LastName = "Green"
            },
            new ApplicationUser
            {
                Id = Guid.Parse("cac52604-bd1c-4161-9e11-2548f3fd0596"),
                UserName = "james.william",
                NormalizedUserName = "JAMES.WILLIAM",
                Email = "james@example.com",
                NormalizedEmail = "JAMES@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd$@00198763.."),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                FirstName = "James",
                LastName = "William"
            },
            new ApplicationUser
            {
                Id = Guid.Parse("aac3fa46-7f85-418f-8081-7492310af74d"),
                UserName = "john.michael",
                NormalizedUserName = "JOHN.MICHAEL",
                Email = "john@example.com",
                NormalizedEmail = "JOHN@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd$@00198763.."),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                FirstName = "John",
                LastName = "Michael"
            },
            new ApplicationUser
            {
                Id = Guid.Parse("cec7091c-75c5-4fbc-98ee-42f6e767c8ed"),
                UserName = "robert.davic",
                NormalizedUserName = "ROBERT.DAVIC",
                Email = "robert@example.com",
                NormalizedEmail = "ROBERT@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd$@00198763.."),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                FirstName = "Robert",
                LastName = "Davic"
            },
            new ApplicationUser
            {
                Id = Guid.Parse("83497d95-ae94-472e-94fe-649fb1477866"),
                UserName = "daniel.andrew",
                NormalizedUserName = "DANIEL.ANDREW",
                Email = "daniel@example.com",
                NormalizedEmail = "DANIEL@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd$@00198763.."),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                FirstName = "Daniel",
                LastName = "Andrew"
            },
            new ApplicationUser
            {
                Id = Guid.Parse("5f3976a9-c1da-44b4-ae1f-f5f402312551"),
                UserName = "ethan.matthew",
                NormalizedUserName = "ETHAN.MATTHEW",
                Email = "ethan@example.com",
                NormalizedEmail = "ETHAN@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Abcd$@00198763.."),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                FirstName = "Ethan",
                LastName = "Matthew"
            }
        };
    }
}