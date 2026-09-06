using SubscriptionManager.Domain.Entities;

namespace SubscriptionManager.Domain.Tests;

public class CustomerTests
{
    [Fact]
    public void Constructor_ValidData_SetsProperties()
    {
        var customer = new Customer("Kevin Dias", "kevin@email.com", "12345678900");

        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal("Kevin Dias", customer.Name);
        Assert.Equal("kevin@email.com", customer.Email);
        Assert.Equal("12345678900", customer.Document);
    }

    [Fact]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Customer(string.Empty, "kevin@email.com", "12345678900"));
    }

    [Fact]
    public void Constructor_WhiteSpaceName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Customer("   ", "kevin@email.com", "12345678900"));
    }

    [Fact]
    public void Constructor_EmptyEmail_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Customer("Kevin Dias", string.Empty, "12345678900"));
    }

    [Fact]
    public void Constructor_EmailWithoutAtSign_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Customer("Kevin Dias", "kevin.email.com", "12345678900"));
    }

    [Fact]
    public void Constructor_EmptyDocument_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Customer("Kevin Dias", "kevin@email.com", string.Empty));
    }
}
