using Moq;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Repositories;

namespace CommonTestUtilities.Repositories;

public class IPlanRepositoryBuilder
{
    private readonly Mock<IPlanRepository> _mock = new();

    public IPlanRepositoryBuilder GetById(Plan plan)
    {
        _mock.Setup(repository => repository.GetByIdAsync(plan.Id)).ReturnsAsync(plan);

        return this;
    }

    public IPlanRepositoryBuilder ExistsActiveWithName(string name)
    {
        _mock.Setup(repository => repository.ExistsActiveWithNameAsync(name)).ReturnsAsync(true);

        return this;
    }

    public IPlanRepositoryBuilder GetAll(params Plan[] plans)
    {
        _mock.Setup(repository => repository.GetAllAsync()).ReturnsAsync(plans);

        return this;
    }

    public IPlanRepository Build() => _mock.Object;
}
