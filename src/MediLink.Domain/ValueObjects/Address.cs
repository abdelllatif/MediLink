namespace MediLink.Domain.ValueObjects;

/// <summary>
/// Address ValueObject - Represents a physical address
/// </summary>
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }

    public Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    public override string ToString() => $"{Street}, {City}, {PostalCode}, {Country}";

    public override bool Equals(object? obj)
    {
        if (obj is not Address address)
            return false;

        return Street == address.Street &&
               City == address.City &&
               State == address.State &&
               PostalCode == address.PostalCode &&
               Country == address.Country;
    }

    public override int GetHashCode() =>
        HashCode.Combine(Street, City, State, PostalCode, Country);
}
