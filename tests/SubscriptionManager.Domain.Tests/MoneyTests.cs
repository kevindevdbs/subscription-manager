using Shouldly;
﻿using SubscriptionManager.Domain.ValueObjects;

namespace SubscriptionManager.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Add_TwoValidAmounts_ReturnsSum()
    {
        var money = new Money(10);

        var result = money.Add(new Money(5));

        result.ShouldBe(new Money(15));
    }

    [Fact]
    public void ApplyPercentage_FractionalResult_RoundsUp()
    {
        var money = new Money(33.33m);
        var result = money.ApplyPercentage(2m);
        result.Amount.ShouldBe(0.67m);
    }

    [Fact]
    public void Constructor_NegativeAmount_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Money(-1));

    }

    [Fact]
    public void Equals_SameAmount_ReturnsTrue()
    {
        new Money(10).ShouldBe(new Money(10));
    }

    [Fact]
    public void Constructor_ZeroAmount_CreatesMoney()
    {
        var money = new Money(0);

        money.Amount.ShouldBe(0m);
    }

    [Fact]
    public void Add_ValidAmount_DoesNotChangeOriginal()
    {
        var money = new Money(10);

        money.Add(new Money(5));

        money.Amount.ShouldBe(10m);
    }

    [Fact]
    public void ApplyPercentage_ZeroPercentage_ReturnsZero()
    {
        var money = new Money(33.33m);

        var result = money.ApplyPercentage(0m);

        result.Amount.ShouldBe(0m);
    }

    [Fact]
    public void ApplyPercentage_HundredPercentage_ReturnsSameAmount()
    {
        var money = new Money(33.33m);

        var result = money.ApplyPercentage(100m);

        result.Amount.ShouldBe(33.33m);
    }

    [Fact]
    public void ApplyPercentage_MidpointResult_RoundsAwayFromZero()
    {
        var money = new Money(1);

        var result = money.ApplyPercentage(2.5m);

        result.Amount.ShouldBe(0.03m);
    }

    [Fact]
    public void ApplyPercentage_NegativePercentage_ThrowsArgumentException()
    {
        var money = new Money(10);

        Should.Throw<ArgumentException>(() => money.ApplyPercentage(-1m));
    }

    [Fact]
    public void Equals_DifferentAmounts_ReturnsFalse()
    {
        new Money(20).ShouldNotBe(new Money(10));
    }

    [Fact]
    public void Add_NullAmount_ThrowsArgumentNullException()
    {
        var money = new Money(10);

        Should.Throw<ArgumentNullException>(() => money.Add(null!));
    }
}
