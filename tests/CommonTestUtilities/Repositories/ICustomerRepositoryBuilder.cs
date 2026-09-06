using Moq;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Repositories;

namespace CommonTestUtilities.Repositories;

public class ICustomerRepositoryBuilder
{
    private readonly Mock<ICustomerRepository> _mock = new();

    public ICustomerRepositoryBuilder GetById(Customer customer)
    {
        _mock.Setup(repository => repository.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

        return this;
    }

    public ICustomerRepositoryBuilder GetAll(params Customer[] customers)
    {
        _mock.Setup(repository => repository.GetAllAsync()).ReturnsAsync(customers);

        return this;
    }

    public ICustomerRepository Build() => _mock.Object;
}
