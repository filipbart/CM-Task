using CM_Task.Application.Abstractions;
using CM_Task.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CM_Task.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(AppDbContext dbContext, IUnitOfWork unitOfWork)
    : RepositoryBase<Order>(dbContext), IOrderRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    public IUnitOfWork UnitOfWork => unitOfWork;

    public new async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _dbContext.Orders
            .Include(o => o.Lines)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
}