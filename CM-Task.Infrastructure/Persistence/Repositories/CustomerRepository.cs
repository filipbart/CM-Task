using CM_Task.Application.Abstractions;
using CM_Task.Domain.Entities;

namespace CM_Task.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(AppDbContext dbContext, IUnitOfWork unitOfWork)
    : RepositoryBase<Customer>(dbContext), ICustomerRepository
{
    public IUnitOfWork UnitOfWork => unitOfWork;
}
