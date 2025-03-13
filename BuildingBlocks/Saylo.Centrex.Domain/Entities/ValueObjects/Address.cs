
namespace Saylo.Centrex.Domain.Entities.ValueObjects;

public class Address
{
    public Address()
    {
    }
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }

    // Constructor
    public Address(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Street cannot be empty.");
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City cannot be empty.");
        if (string.IsNullOrWhiteSpace(state)) throw new ArgumentException("State cannot be empty.");
        if (string.IsNullOrWhiteSpace(postalCode)) throw new ArgumentException("PostalCode cannot be empty.");
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country cannot be empty.");

        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    // Equality based on the value of the properties (not reference)
    public override bool Equals(object obj)
    {
        if (obj is Address address)
        {
            return Street == address.Street &&
                   City == address.City &&
                   State == address.State &&
                   PostalCode == address.PostalCode &&
                   Country == address.Country;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, State, PostalCode, Country);
    }

     public override string ToString()
    {
        return $"{Street}, {City}, {State}, {PostalCode}, {Country}";
    }
}