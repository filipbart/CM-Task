using CM_Task.Domain.Entities;

namespace CM_Task.Application.Abstractions;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
}