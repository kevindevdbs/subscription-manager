using Bogus;
using SubscriptionManager.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class ContractBuilder
{
    public static Contract Build(Guid? customerId = null, Guid? planId = null)
    {
        return new Faker<Contract>()
            .CustomInstantiator(faker => new Contract(
                customerId ?? Guid.NewGuid(),
                planId ?? Guid.NewGuid(),
                faker.Date.Recent()));
    }
}
