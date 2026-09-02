using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.ValueObjects;

namespace SubscriptionManager.Domain.Tests.Builders;

public class InvoiceBuilder
{
    public static Invoice Build()
    {
        return new Invoice(Guid.NewGuid(), new Money(100), DateTime.Now.AddDays(30));
    }
}
