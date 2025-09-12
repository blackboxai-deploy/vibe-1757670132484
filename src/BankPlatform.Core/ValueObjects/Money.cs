using System.ComponentModel.DataAnnotations.Schema;

namespace BankPlatform.Core.ValueObjects;

/// <summary>
/// Money value object for handling currency amounts
/// Ensures consistent money handling across the application
/// </summary>
[ComplexType]
public class Money : IEquatable<Money>, IComparable<Money>
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    protected Money() 
    { 
        Currency = string.Empty;
    }

    public Money(decimal amount, string currency = "GHS")
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty", nameof(currency));
            
        Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency = "GHS") => new(0, currency);
    
    public static Money FromGHS(decimal amount) => new(amount, "GHS");

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        return new Money(Amount * factor, Currency);
    }

    public Money Divide(decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException("Cannot divide money by zero");
            
        return new Money(Amount / divisor, Currency);
    }

    public bool IsPositive => Amount > 0;
    public bool IsNegative => Amount < 0;
    public bool IsZero => Amount == 0;

    public Money Abs() => new(Math.Abs(Amount), Currency);

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot operate on different currencies: {Currency} and {other.Currency}");
    }

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money money, decimal factor) => money.Multiply(factor);
    public static Money operator /(Money money, decimal divisor) => money.Divide(divisor);
    
    public static bool operator ==(Money? left, Money? right) => EqualityComparer<Money>.Default.Equals(left, right);
    public static bool operator !=(Money? left, Money? right) => !(left == right);
    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;
    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;
    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;
    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj) => obj is Money other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public int CompareTo(Money? other)
    {
        if (other is null) return 1;
        EnsureSameCurrency(other);
        return Amount.CompareTo(other.Amount);
    }

    public override string ToString() => $"{Currency} {Amount:N2}";
    
    public string ToString(string format) => $"{Currency} {Amount.ToString(format)}";
}