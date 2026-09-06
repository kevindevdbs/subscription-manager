using Bogus;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.ValueObjects;

namespace CommonTestUtilities.Entities;

public class InvoiceBuilder
{
    public static Invoice Build(Guid? contractId = null, DateTime? referenceMonth = null, decimal amount = 100)
    {
        var month = referenceMonth ?? new DateTime(2026, 9, 1);

        return new Faker<Invoice>()
            .CustomInstantiator(_ => new Invoice(
                contractId ?? Guid.NewGuid(),
                new Money(amount),
                month.AddDays(9),
                month));
    }
}
