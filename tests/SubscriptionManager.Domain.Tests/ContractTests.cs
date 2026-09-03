using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Enums;
using SubscriptionManager.Domain.Tests.Builders;

namespace SubscriptionManager.Domain.Tests;

public class ContractTests
{
    [Fact]
    public void Constructor_ValidData_SetsStatusToActive()
    {
        var contract = ContractBuilder.Build();

        Assert.Equal(ContractStatus.Active, contract.Status);
    }

    [Fact]
    public void Constructor_EmptyCustomerId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Contract(Guid.Empty, Guid.NewGuid(), DateTime.Now));
    }

    [Fact]
    public void Constructor_EmptyPlanId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Contract(Guid.NewGuid(), Guid.Empty, DateTime.Now));
    }

    [Fact]
    public void Suspend_ActiveContract_ChangesStatusToSuspended()
    {
        var contract = ContractBuilder.Build();

        contract.Suspend();

        Assert.Equal(ContractStatus.Suspended, contract.Status);
    }

    [Fact]
    public void Suspend_SuspendedContract_ThrowsInvalidOperationException()
    {
        var contract = ContractBuilder.Build();
        contract.Suspend();

        Assert.Throws<InvalidOperationException>(() => contract.Suspend());
    }

    [Fact]
    public void Suspend_CancelledContract_ThrowsInvalidOperationException()
    {
        var contract = ContractBuilder.Build();
        contract.Cancel(DateTime.Now);

        Assert.Throws<InvalidOperationException>(() => contract.Suspend());
    }

    [Fact]
    public void Cancel_ActiveContract_ChangesStatusToCancelled()
    {
        var contract = ContractBuilder.Build();

        contract.Cancel(DateTime.Now);

        Assert.Equal(ContractStatus.Cancelled, contract.Status);
    }

    [Fact]
    public void Cancel_ActiveContract_SetsEndDate()
    {
        var contract = ContractBuilder.Build();
        var endDate = DateTime.Now;

        contract.Cancel(endDate);

        Assert.Equal(endDate, contract.EndDate);
    }

    [Fact]
    public void Cancel_SuspendedContract_ChangesStatusToCancelled()
    {
        var contract = ContractBuilder.Build();
        contract.Suspend();

        contract.Cancel(DateTime.Now);

        Assert.Equal(ContractStatus.Cancelled, contract.Status);
    }

    [Fact]
    public void Cancel_CancelledContract_ThrowsInvalidOperationException()
    {
        var contract = ContractBuilder.Build();
        contract.Cancel(DateTime.Now);

        Assert.Throws<InvalidOperationException>(() => contract.Cancel(DateTime.Now));
    }

    [Fact]
    public void Reactivate_SuspendedContract_ChangesStatusToActive()
    {
        var contract = ContractBuilder.Build();
        contract.Suspend();

        contract.Reactivate();

        Assert.Equal(ContractStatus.Active, contract.Status);
    }

    [Fact]
    public void Reactivate_ActiveContract_ThrowsInvalidOperationException()
    {
        var contract = ContractBuilder.Build();

        Assert.Throws<InvalidOperationException>(() => contract.Reactivate());
    }

    [Fact]
    public void Reactivate_CancelledContract_ThrowsInvalidOperationException()
    {
        var contract = ContractBuilder.Build();
        contract.Cancel(DateTime.Now);

        Assert.Throws<InvalidOperationException>(() => contract.Reactivate());
    }
}
