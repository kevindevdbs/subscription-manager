using SubscriptionManager.Domain.ValueObjects;


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
            throw new ArgumentException("O nome não pode ser nulo ou conter apenas espaços em branco.", nameof(name));
        }

        if (monthlyPrice == null)
        {
            throw new ArgumentNullException(nameof(monthlyPrice), "O preço mensal não pode ser nulo.");
        }

        if (monthlyPrice.Amount <= 0)
        {
            throw new ArgumentException("O preço mensal deve ser maior que zero.", nameof(monthlyPrice));
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
            throw new InvalidOperationException("O plano já está desativado.");
        }

        IsActive = false;
    }
}
