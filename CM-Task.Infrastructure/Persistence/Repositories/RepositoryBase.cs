using CM_Task.Application.Abstractions;
using CM_Task.Domain.Entities;

namespace CM_Task.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<T>(AppDbContext dbContext) : ICustomRepository<T>
    where T : Entity
{
    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return dbContext.Set<T>().FindAsync([id], ct).AsTask();
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await dbContext.Set<T>().AddAsync(entity, ct);
    }

    public void Update(T entity)
    {
        dbContext.Set<T>().Update(entity);
    }
}