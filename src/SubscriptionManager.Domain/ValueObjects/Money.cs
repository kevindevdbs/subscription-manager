namespace SubscriptionManager.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; private set; }

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
        Amount = amount;
    }

    public Money Add(Money money)
    {
        decimal amountSum = Amount + money.Amount;
        return new Money(amountSum);
    }

    public Money ApplyPercentage(decimal percentage)
    {
        decimal amountPercentage = Math.Round((Amount / 100) * percentage, 2, MidpointRounding.AwayFromZero);
        return new Money(amountPercentage);
    }

    public override string ToString()
    {
        return Amount.ToString("C");
    }
}
