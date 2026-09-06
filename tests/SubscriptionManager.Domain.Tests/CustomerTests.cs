using Shouldly;
using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Domain.Tests;

public class CustomerTests
{
    [Fact]
    public void Constructor_ValidData_SetsProperties()
    {
        var customer = new Customer("Kevin Dias", "kevin@email.com", "12345678900");

        customer.Id.ShouldNotBe(Guid.Empty);
        customer.Name.ShouldBe("Kevin Dias");
        customer.Email.ShouldBe("kevin@email.com");
        customer.Document.ShouldBe("12345678900");
    }

    [Fact]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Customer(string.Empty, "kevin@email.com", "12345678900"));
    }

    [Fact]
    public void Constructor_WhiteSpaceName_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Customer("   ", "kevin@email.com", "12345678900"));
    }

    [Fact]
    public void Constructor_EmptyEmail_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Customer("Kevin Dias", string.Empty, "12345678900"));
    }

    [Fact]
    public void Constructor_EmailWithoutAtSign_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Customer("Kevin Dias", "kevin.email.com", "12345678900"));
    }

    [Fact]
    public void Constructor_EmptyDocument_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Customer("Kevin Dias", "kevin@email.com", string.Empty));
    }
}
