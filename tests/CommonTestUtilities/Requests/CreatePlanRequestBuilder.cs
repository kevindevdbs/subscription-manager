using Bogus;
using SubscriptionManager.Application.DTOs.Plans;

namespace CommonTestUtilities.Requests;

public class CreatePlanRequestBuilder
{
    public static CreatePlanRequest Build()
    {
        return new Faker<CreatePlanRequest>()
            .CustomInstantiator(faker => new CreatePlanRequest(
                faker.Commerce.ProductName(),
                faker.Random.Decimal(10, 500)));
    }
}
