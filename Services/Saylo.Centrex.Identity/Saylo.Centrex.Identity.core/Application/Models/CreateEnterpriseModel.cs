namespace Saylo.Centrex.Identity.Core.Application.Models;

public class CreateEnterpriseModel
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Siret { get; set; }
    public AddressDto MainAddress { get; set; }
    public AddressDto? SecondAddress { get; set; }
}

public class AddressDto
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}