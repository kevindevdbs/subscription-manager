using Shouldly;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Enums;
using SubscriptionManager.Domain.Tests.Builders;
using SubscriptionManager.Domain.Exceptions;

namespace SubscriptionManager.Domain.Tests;

public class ContractTests
{
    [Fact]
    public void Constructor_ValidData_SetsStatusToActive()
    {
        var contract = ContractBuilder.Build();

        contract.Status.ShouldBe(ContractStatus.Active);
    }

    [Fact]
    public void Constructor_EmptyCustomerId_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Contract(Guid.Empty, Guid.NewGuid(), DateTime.Now));
    }

    [Fact]
    public void Constructor_EmptyPlanId_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Contract(Guid.NewGuid(), Guid.Empty, DateTime.Now));
    }

    [Fact]
    public void Suspend_ActiveContract_ChangesStatusToSuspended()
    {
        var contract = ContractBuilder.Build();

        contract.Suspend();

        contract.Status.ShouldBe(ContractStatus.Suspended);
    }

    [Fact]
    public void Suspend_SuspendedContract_ThrowsConflictException()
    {
        var contract = ContractBuilder.Build();
        contract.Suspend();

        Should.Throw<ConflictException>(() => contract.Suspend());
    }

    [Fact]
    public void Suspend_CancelledContract_ThrowsConflictException()
    {
        var contract = ContractBuilder.Build();
        contract.Cancel(DateTime.Now);

        Should.Throw<ConflictException>(() => contract.Suspend());
    }

    [Fact]
    public void Cancel_ActiveContract_ChangesStatusToCancelled()
    {
        var contract = ContractBuilder.Build();

        contract.Cancel(DateTime.Now);

        contract.Status.ShouldBe(ContractStatus.Cancelled);
    }

    [Fact]
    public void Cancel_ActiveContract_SetsEndDate()
    {
        var contract = ContractBuilder.Build();
        var endDate = DateTime.Now;

        contract.Cancel(endDate);

        contract.EndDate.ShouldBe(endDate);
    }

    [Fact]
    public void Cancel_SuspendedContract_ChangesStatusToCancelled()
    {
        var contract = ContractBuilder.Build();
        contract.Suspend();

        contract.Cancel(DateTime.Now);

        contract.Status.ShouldBe(ContractStatus.Cancelled);
    }

    [Fact]
    public void Cancel_CancelledContract_ThrowsConflictException()
    {
        var contract = ContractBuilder.Build();
        contract.Cancel(DateTime.Now);

        Should.Throw<ConflictException>(() => contract.Cancel(DateTime.Now));
    }

    [Fact]
    public void Reactivate_SuspendedContract_ChangesStatusToActive()
    {
        var contract = ContractBuilder.Build();
        contract.Suspend();

        contract.Reactivate();

        contract.Status.ShouldBe(ContractStatus.Active);
    }

    [Fact]
    public void Reactivate_ActiveContract_ThrowsConflictException()
    {
        var contract = ContractBuilder.Build();

        Should.Throw<ConflictException>(() => contract.Reactivate());
    }

    [Fact]
    public void Reactivate_CancelledContract_ThrowsConflictException()
    {
        var contract = ContractBuilder.Build();
        contract.Cancel(DateTime.Now);

        Should.Throw<ConflictException>(() => contract.Reactivate());
    }
}
