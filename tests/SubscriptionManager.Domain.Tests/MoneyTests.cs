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

    [Fact]
    public void Constructor_ZeroAmount_CreatesMoney()
    {
        var money = new Money(0);

        Assert.Equal(0m, money.Amount);
    }

    [Fact]
    public void Add_ValidAmount_DoesNotChangeOriginal()
    {
        var money = new Money(10);

        money.Add(new Money(5));

        Assert.Equal(10m, money.Amount);
    }

    [Fact]
    public void ApplyPercentage_ZeroPercentage_ReturnsZero()
    {
        var money = new Money(33.33m);

        var result = money.ApplyPercentage(0m);

        Assert.Equal(0m, result.Amount);
    }

    [Fact]
    public void ApplyPercentage_HundredPercentage_ReturnsSameAmount()
    {
        var money = new Money(33.33m);

        var result = money.ApplyPercentage(100m);

        Assert.Equal(33.33m, result.Amount);
    }

    [Fact]
    public void ApplyPercentage_MidpointResult_RoundsAwayFromZero()
    {
        var money = new Money(1);

        var result = money.ApplyPercentage(2.5m);

        Assert.Equal(0.03m, result.Amount);
    }

    [Fact]
    public void ApplyPercentage_NegativePercentage_ThrowsArgumentException()
    {
        var money = new Money(10);

        Assert.Throws<ArgumentException>(() => money.ApplyPercentage(-1m));
    }

    [Fact]
    public void Equals_DifferentAmounts_ReturnsFalse()
    {
        Assert.NotEqual(new Money(10), new Money(20));
    }

    [Fact]
    public void Add_NullAmount_ThrowsArgumentNullException()
    {
        var money = new Money(10);

        Assert.Throws<ArgumentNullException>(() => money.Add(null!));
    }
}
