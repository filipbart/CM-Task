using CM_Task.Domain.Entities;

namespace CM_Task.Application.Abstractions;

public interface IRepository;

public interface IRepository<T> : ICustomRepository<T> where T : Entity
{
    IUnitOfWork UnitOfWork { get; }
}

public interface ICustomRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
}