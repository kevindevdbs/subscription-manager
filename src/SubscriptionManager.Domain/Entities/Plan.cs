using SubscriptionManager.Domain.ValueObjects;

using SubscriptionManager.Domain.Resources;

namespace SubscriptionManager.Domain.Entities;

public class Plan
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Money MonthlyPrice { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private Plan()
    {

    }

    public Plan(string name, Money monthlyPrice)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(DomainMessages.NameRequired, nameof(name));
        }

        if (monthlyPrice == null)
        {
            throw new ArgumentNullException(nameof(monthlyPrice));
        }

        if (monthlyPrice.Amount <= 0)
        {
            throw new ArgumentException(DomainMessages.PlanMonthlyPriceMustBeGreaterThanZero, nameof(monthlyPrice));
        }

        Id = Guid.NewGuid();
        Name = name;
        MonthlyPrice = monthlyPrice;
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException(DomainMessages.PlanAlreadyInactive);
        }

        IsActive = false;
    }
}
