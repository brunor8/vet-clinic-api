namespace VetClinic.Application.Common.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChanges(CancellationToken cancellationToken = default);
}