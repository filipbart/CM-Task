using CM_Task.Application.Abstractions;
using CM_Task.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CM_Task.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var entities = typeof(Customer).Assembly.GetTypes().Where(t => !t.IsAbstract)
            .Where(t => t.IsSubclassOf(typeof(Entity)));

        foreach (var entity in entities)
        {
            builder.Entity(entity);
            foreach (var propertyInfo in entity.GetProperties())
            {
                if (propertyInfo.PropertyType.IsEnum)
                {
                    builder.Entity(entity).Property(propertyInfo.Name).HasConversion<string>().HasMaxLength(100);
                }
            }
        }

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        foreach (var type in builder.Model.GetEntityTypes())
        {
            if (type.IsOwned())
                continue;

            var p = type.FindPrimaryKey()?.Properties.FirstOrDefault(t => t.ValueGenerated != ValueGenerated.Never);

            p?.ValueGenerated = ValueGenerated.Never;
        }
    }
}