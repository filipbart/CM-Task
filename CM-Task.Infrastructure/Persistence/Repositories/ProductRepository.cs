using CM_Task.Application.Abstractions;
using CM_Task.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CM_Task.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(AppDbContext dbContext, IUnitOfWork unitOfWork)
    : RepositoryBase<Product>(dbContext), IProductRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    public IUnitOfWork UnitOfWork => unitOfWork;

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
        => await _dbContext.Products.AsNoTracking().ToListAsync(ct);

    public Task<Dictionary<Guid, Product>> GetByIdsAsync(IReadOnlyList<Guid> ids, CancellationToken ct = default)
    {
        return _dbContext.Products.Where(p => ids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        await _dbContext.Products.AnyAsync(p => p.Id == id, ct);
}