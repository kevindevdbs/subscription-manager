using Bogus;
using SubscriptionManager.Application.DTOs.Customers;

namespace CommonTestUtilities.Requests;

public class CreateCustomerRequestBuilder
{
    public static CreateCustomerRequest Build()
    {
        return new Faker<CreateCustomerRequest>()
            .CustomInstantiator(faker => new CreateCustomerRequest(
                faker.Person.FullName,
                faker.Internet.Email(),
                faker.Random.ReplaceNumbers("###########")));
    }
}
