namespace CM_Task.Domain.Entities;

public sealed class Order : Entity
{
    private Order()
    {
    }

    private readonly List<OrderLine> _lines = [];

    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; }
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    public decimal TotalValue { get; private set; }
    public string? AppliedDiscountName { get; private set; }


    public static Order Create(Customer customer, DateTime createdAt) => new()
    {
        CustomerId = customer.Id,
        Customer = customer,
        CreatedAt = createdAt
    };

    public void AddLine(OrderLine line) => _lines.Add(line);

    public void ApplyPricing(decimal totalValue, string? discountName)
    {
        TotalValue = totalValue;
        AppliedDiscountName = discountName;
    }
}