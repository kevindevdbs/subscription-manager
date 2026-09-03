using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Domain.Tests.Builders;

public class ContractBuilder
{
    public static Contract Build()
    {
        return new Contract(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now);
    }
}
