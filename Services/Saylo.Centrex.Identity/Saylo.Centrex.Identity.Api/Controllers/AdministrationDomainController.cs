using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saylo.Centrex.Application.Common.Queries;
using Saylo.Centrex.Identity.Core.Application.Commands.AdministrationDomains.Enterprises;
using Saylo.Centrex.Identity.Core.Application.Commands.AdministrationDomains.ServiceProviders;
using Saylo.Centrex.Identity.Core.Application.DTOs;
using Saylo.Centrex.Identity.Core.Application.Models;
using Saylo.Centrex.Identity.Core.Application.Queries.AdministrationDomains;
using Saylo.Centrex.Identity.Core.Application.Queries.Identity;
using Saylo.Centrex.Infrastructure.Identity;
using Saylo.Centrex.Infrastructure.Web.Apis;

namespace Saylo.Centrex.Identity.Api.Controllers;
public class AdministrationDomainController : ApiControllerBase
{
    public AdministrationDomainController(
        IMapper mapper, 
        IMediator dispatcher, 
        ILogger<AdministrationDomainController> logger) : base(dispatcher, mapper, logger)
    {
    }
    
    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpPost("create-enterprise")]
    public async Task<IActionResult> CreateEnterprise([FromBody]CreateEnterpriseModel model)
    {
        var result = await SendCommandAsync<CreateEnterpriseCommand, CreateEnterpriseModel, Guid>(model);
        return Ok(result);
    }
    
    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpPost("create-service-provider")]
    public async Task<IActionResult> CreateServiceProvider([FromBody]CreateServiceProviderModel model)
    {
        var result = await SendCommandAsync<CreateServiceProviderCommand, CreateServiceProviderModel, Guid>(model);
        return Ok(result);
    }
    
    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpPut("{id:guid}/update-service-provider")]
    public async Task<IActionResult> UpdateServiceProvider([FromRoute] Guid id, [FromBody]UpdateServiceProviderModel model)
    {
        if (id != model.Id) return BadRequest();
        var result = await SendCommandAsync<UpdateServiceProviderCommand, UpdateServiceProviderModel, Boolean>(model);
        return Ok(result);
    }
    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpGet("current-tenant")]
    public async Task<ActionResult<AdministrationDomainDto>> GetCurrentTenant()
    {
        var request = new GetAdministrationDomainByIdQuery() ;
        var result = await SendQueryAsync<GetAdministrationDomainByIdQuery, AdministrationDomainDto>(request);
        return Ok(result);
    }
    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpGet("tenants")]
    public async Task<ActionResult<PagedResult<AdministrationDomainDto>>> GetTenants(string? search, int page, int size)
    {
        var request = new GetAllAdministrationDomainsByPageQuery { Page = page, Size = size, Search = search};
        var result = await SendQueryAsync<GetAllAdministrationDomainsByPageQuery, PagedResult<AdministrationDomainDto>>(request);
        return Ok(result);
    }
}