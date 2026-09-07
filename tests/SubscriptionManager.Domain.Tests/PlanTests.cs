using Shouldly;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Tests.Builders;
using SubscriptionManager.Domain.ValueObjects;
using SubscriptionManager.Domain.Exceptions;

namespace SubscriptionManager.Domain.Tests;

public class PlanTests
{
    [Fact]
    public void Constructor_ValidData_SetsPlanAsActive()
    {
        var plan = PlanBuilder.Build();

        plan.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Plan(string.Empty, new Money(100)));
    }

    [Fact]
    public void Constructor_WhiteSpaceName_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Plan("   ", new Money(100)));
    }

    [Fact]
    public void Constructor_NullMonthlyPrice_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => new Plan("Plano Mensal", null!));
    }

    [Fact]
    public void Constructor_ZeroMonthlyPrice_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Plan("Plano Mensal", new Money(0)));
    }

    [Fact]
    public void Deactivate_ActivePlan_SetsPlanAsInactive()
    {
        var plan = PlanBuilder.Build();

        plan.Deactivate();

        plan.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Deactivate_InactivePlan_ThrowsConflictException()
    {
        var plan = PlanBuilder.Build();
        plan.Deactivate();

        Should.Throw<ConflictException>(() => plan.Deactivate());
    }
}
