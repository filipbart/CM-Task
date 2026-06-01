using CM_Task.Domain.Entities;
using CM_Task.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CM_Task.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Customers.AnyAsync()) return;

        var customers = new[]
        {
            Customer.Create("Alice Johnson", Region.Usa),
            Customer.Create("Erik Müller", Region.Europe),
            Customer.Create("Yuki Tanaka", Region.Asia),
            Customer.Create("Maria Santos", Region.Europe),
            Customer.Create("James Carter", Region.Usa)
        };

        await db.Customers.AddRangeAsync(customers);
        await db.SaveChangesAsync();
    }
}