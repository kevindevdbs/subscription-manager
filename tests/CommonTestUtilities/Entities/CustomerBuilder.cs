using Bogus;
using SubscriptionManager.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class CustomerBuilder
{
    public static Customer Build()
    {
        return new Faker<Customer>()
            .CustomInstantiator(faker => new Customer(
                faker.Person.FullName,
                faker.Internet.Email(),
                faker.Random.ReplaceNumbers("###########")));
    }
}
