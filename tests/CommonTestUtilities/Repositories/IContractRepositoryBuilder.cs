using Moq;
using SubscriptionManager.Domain.Entities;
using SubscriptionManager.Domain.Repositories;

namespace CommonTestUtilities.Repositories;

public class IContractRepositoryBuilder
{
    private readonly Mock<IContractRepository> _mock = new();

    public IContractRepositoryBuilder GetById(Contract contract)
    {
        _mock.Setup(repository => repository.GetByIdAsync(contract.Id)).ReturnsAsync(contract);

        return this;
    }

    public IContractRepositoryBuilder GetActiveContracts(params Contract[] contracts)
    {
        _mock.Setup(repository => repository.GetActiveContractsAsync()).ReturnsAsync(contracts);

        return this;
    }

    public IContractRepositoryBuilder GetAll(params Contract[] contracts)
    {
        _mock.Setup(repository => repository.GetAllAsync()).ReturnsAsync(contracts);

        return this;
    }

    public IContractRepository Build() => _mock.Object;
}
