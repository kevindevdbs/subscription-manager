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
            throw new ArgumentException("O nome não pode ser nulo ou conter apenas espaços em branco.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("O e-mail não pode ser nulo ou conter apenas espaços em branco.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(document))
        {
            throw new ArgumentException("O documento não pode ser nulo ou conter apenas espaços em branco.", nameof(document));
        }

        if (!email.Contains("@"))
        {
            throw new ArgumentException("O email deve conter o caractere '@'.", nameof(email));
        }

        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Document = document;
        CreatedAt = DateTime.UtcNow;
    }

}
