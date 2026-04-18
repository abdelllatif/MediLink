namespace MediLink.Domain.ValueObjects;

/// <summary>
/// Money ValueObject - Represents a monetary amount with currency
/// </summary>
public class Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "MAD")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        Amount = amount;
        Currency = currency;
    }

    public override string ToString() => $"{Amount} {Currency}";

    public override bool Equals(object? obj)
    {
        if (obj is not Money money)
            return false;

        return Amount == money.Amount && Currency == money.Currency;
    }

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");

        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static Money operator *(Money m, decimal multiplier) =>
        new(m.Amount * multiplier, m.Currency);

    public static Money operator /(Money m, decimal divisor)
    {
        if (divisor == 0)
            throw new InvalidOperationException("Cannot divide by zero");

        return new Money(m.Amount / divisor, m.Currency);
    }
}
