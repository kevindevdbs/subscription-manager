using Bogus;
using SubscriptionManager.Application.DTOs.Plans;

namespace CommonTestUtilities.Requests;

public class CreatePlanRequestBuilder
{
    public static CreatePlanRequest Build()
    {
        return new Faker<CreatePlanRequest>()
            .CustomInstantiator(faker => new CreatePlanRequest(
                // Sufixo porque o vocabulário do Bogus é pequeno e a API recusa
                // nome de plano ativo repetido — dois sorteios colidiam e vinha 409.
                $"{faker.Commerce.ProductName()} {Guid.NewGuid().ToString("N")[..12]}",
                faker.Random.Decimal(10, 500)));
    }
}
