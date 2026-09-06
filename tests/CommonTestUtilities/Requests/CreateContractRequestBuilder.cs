using Bogus;
using SubscriptionManager.Application.DTOs.Contracts;

namespace CommonTestUtilities.Requests;

public class CreateContractRequestBuilder
{
    public static CreateContractRequest Build(Guid? customerId = null, Guid? planId = null)
    {
        return new Faker<CreateContractRequest>()
            .CustomInstantiator(faker => new CreateContractRequest(
                customerId ?? Guid.NewGuid(),
                planId ?? Guid.NewGuid(),
                faker.Date.Recent()));
    }
}
