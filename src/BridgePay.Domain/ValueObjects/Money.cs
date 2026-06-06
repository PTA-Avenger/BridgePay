namespace BridgePay.Domain.ValueObjects;

using System;
using BridgePay.Domain.Enums;
using BridgePay.Domain.Exceptions;

/// <summary>
/// Value object representing a monetary amount in a specific currency.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    /// <summary>
    /// Gets the monetary amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the ISO currency.
    /// </summary>
    public Currency Currency { get; }

    /// <summary>
    /// Initializes a new instance of the Money class.
    /// </summary>
    public Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Adds two Money objects of the same currency.
    /// </summary>
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException("Cannot add amounts with different currencies.");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    /// <summary>
    /// Subtracts one Money object from another of the same currency.
    /// </summary>
    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException("Cannot subtract amounts with different currencies.");
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    /// <summary>
    /// Checks for equality between two Money objects.
    /// </summary>
    public static bool operator ==(Money? a, Money? b) => a?.Equals(b) ?? b is null;

    /// <summary>
    /// Checks for inequality between two Money objects.
    /// </summary>
    public static bool operator !=(Money? a, Money? b) => !(a == b);

    /// <summary>
    /// Checks if this instance equals another Money instance.
    /// </summary>
    public bool Equals(Money? other)
    {
        return other is not null && Amount == other.Amount && Currency == other.Currency;
    }

    /// <summary>
    /// Checks if this instance equals another object.
    /// </summary>
    public override bool Equals(object? obj) => Equals(obj as Money);

    /// <summary>
    /// Generates a hash code for this Money instance.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    /// <summary>
    /// Returns a string representation of the Money instance.
    /// </summary>
    public override string ToString() => $"{Amount:N2} {Currency}";
}
