using VetClinic.Application.Common.Abstractions;

namespace VetClinic.Application.UnitTests.Fakes;
public sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCalls {get; private set;} 

    public Task<int> SaveChanges(CancellationToken cancellationToken = default)
    {
        SaveChangesCalls++;
        return Task.FromResult(1);
    }
}