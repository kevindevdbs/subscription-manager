using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.ValueObjects;

namespace SubscriptionManager.Domain.Tests.Builders;

public class PlanBuilder
{
    public static Plan Build()
    {
        return new Plan("Plano Mensal", new Money(100));
    }
}
