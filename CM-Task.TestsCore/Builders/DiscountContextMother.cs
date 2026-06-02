using CM_Task.Application.Discounts;
using CM_Task.Domain.Entities;

namespace CM_Task.TestsCore.Builders;

public static class DiscountContextMother
{
    public static DiscountContext Default(decimal price = 100m, int qty = 1)
    {
        var customer = CustomerMother.Usa();
        var order = OrderMother.ForCustomer(customer);
        var lines = new[]
        {
            OrderLine.Create(order.Id, Guid.NewGuid(), "Product", qty, price)
        };
        return new DiscountContext(lines, customer, new DateOnly(2026, 6, 15));
    }

    public static DiscountContext WithDate(DateOnly date, decimal price = 100m, int qty = 1)
    {
        var customer = CustomerMother.Usa();
        var order = OrderMother.ForCustomer(customer);
        var lines = new[]
        {
            OrderLine.Create(order.Id, Guid.NewGuid(), "Product", qty, price)
        };
        return new DiscountContext(lines, customer, date);
    }

    public static DiscountContext WithLines(
        DateOnly date,
        Customer customer,
        params (decimal price, int qty)[] lines)
    {
        var order = OrderMother.ForCustomer(customer);
        var orderLines = lines
            .Select(l => OrderLine.Create(
                order.Id, Guid.NewGuid(), "Product", l.qty, l.price))
            .ToList();
        return new DiscountContext(orderLines, customer, date);
    }

    public static DateOnly BlackFriday2025 => new(2025, 11, 28);
    public static DateOnly BlackFriday2026 => new(2026, 11, 27);
    public static DateOnly PolishHoliday => new(2025, 11, 11);
    public static DateOnly RegularDay => new(2026, 6, 15);
}