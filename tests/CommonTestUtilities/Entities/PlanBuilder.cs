using Bogus;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.ValueObjects;

namespace CommonTestUtilities.Entities;

public class PlanBuilder
{
    public static Plan Build(decimal? monthlyPrice = null)
    {
        return new Faker<Plan>()
            .CustomInstantiator(faker => new Plan(
                faker.Commerce.ProductName(),
                new Money(monthlyPrice ?? faker.Random.Decimal(10, 500))));
    }
}
