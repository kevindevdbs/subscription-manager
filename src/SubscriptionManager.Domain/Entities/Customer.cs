using SubscriptionManager.Domain.Resources;

namespace SubscriptionManager.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Document { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Customer()
    {

    }

    public Customer(string name, string email, string document)
    {

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(DomainMessages.NameRequired, nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(DomainMessages.EmailRequired, nameof(email));
        }

        if (string.IsNullOrWhiteSpace(document))
        {
            throw new ArgumentException(DomainMessages.DocumentRequired, nameof(document));
        }

        if (!email.Contains("@"))
        {
            throw new ArgumentException(DomainMessages.EmailMustContainAtSign, nameof(email));
        }

        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Document = document;
        CreatedAt = DateTime.UtcNow;
    }

}
