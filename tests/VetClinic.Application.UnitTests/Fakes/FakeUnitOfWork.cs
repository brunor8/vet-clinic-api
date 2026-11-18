using VetClinic.Application.Common.Abstractions;

namespace VetClinic.Application.UnitTests.Fakes;
public sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCalls { get; private set; }
    public Exception? ExceptionToThrow { get; set; }

    public Task<int> SaveChanges(CancellationToken cancellationToken = default)
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        SaveChangesCalls++;
        return Task.FromResult(1);
    }
}