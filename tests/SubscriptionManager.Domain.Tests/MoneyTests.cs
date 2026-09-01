using SubscriptionManager.Domain.ValueObjects;

namespace SubscriptionManager.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Add_TwoValidAmounts_ReturnsSum()
    {
        var money = new Money(10);

        var result = money.Add(new Money(5));

        Assert.Equal(new Money(15), result);
    }

    [Fact]
    public void ApplyPercentage_FractionalResult_RoundsUp()
    {
        var money = new Money(33.33m);
        var result = money.ApplyPercentage(2m);
        Assert.Equal(0.67m, result.Amount);
    }

    [Fact]
    public void Constructor_NegativeAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Money(-1));

    }

    [Fact]
    public void Equals_SameAmount_ReturnsTrue()
    {
        Assert.Equal(new Money(10), new Money(10));
    }
}
