using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Tests.Builders;
using SubscriptionManager.Domain.ValueObjects;

namespace SubscriptionManager.Domain.Tests;

public class PlanTests
{
    [Fact]
    public void Constructor_ValidData_SetsPlanAsActive()
    {
        var plan = PlanBuilder.Build();

        Assert.True(plan.IsActive);
    }

    [Fact]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Plan(string.Empty, new Money(100)));
    }

    [Fact]
    public void Constructor_WhiteSpaceName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Plan("   ", new Money(100)));
    }

    [Fact]
    public void Constructor_NullMonthlyPrice_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Plan("Plano Mensal", null!));
    }

    [Fact]
    public void Constructor_ZeroMonthlyPrice_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Plan("Plano Mensal", new Money(0)));
    }

    [Fact]
    public void Deactivate_ActivePlan_SetsPlanAsInactive()
    {
        var plan = PlanBuilder.Build();

        plan.Deactivate();

        Assert.False(plan.IsActive);
    }

    [Fact]
    public void Deactivate_InactivePlan_ThrowsInvalidOperationException()
    {
        var plan = PlanBuilder.Build();
        plan.Deactivate();

        Assert.Throws<InvalidOperationException>(() => plan.Deactivate());
    }
}
