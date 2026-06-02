using CM_Task.Domain.Entities;

namespace CM_Task.TestsCore.Builders;

public static class OrderMother
{
    public static Order Default()
    {
        var customer = CustomerMother.Usa();
        return Order.Create(customer, DateTime.UtcNow);
    }

    public static Order ForCustomer(Customer customer) =>
        Order.Create(customer, DateTime.UtcNow);

    public static Order WithLines(Customer customer, params (Product product, int qty)[] lines)
    {
        var order = Order.Create(customer, DateTime.UtcNow);
        foreach (var (product, qty) in lines)
        {
            order.AddLine(OrderLine.Create(
                order.Id, product.Id, product.Name, qty, product.Price));
        }
        return order;
    }
}