using CM_Task.Domain.Entities;

namespace CM_Task.Application.Abstractions;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
    Task<Dictionary<Guid, Product>> GetByIdsAsync(IReadOnlyList<Guid> ids, CancellationToken ct = default);
}