namespace CM_Task.Domain.Entities;

public sealed class OrderLine : Entity
{
    private OrderLine()
    {
    }


    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private init; }
    public decimal UnitPrice { get; private init; }


    public static OrderLine Create(
        Guid orderId, Guid productId, string productName,
        int quantity, decimal unitPrice) => new()
    {
        OrderId = orderId,
        ProductId = productId,
        ProductName = productName,
        Quantity = quantity,
        UnitPrice = unitPrice
    };

    public decimal LineTotal => Quantity * UnitPrice;
}