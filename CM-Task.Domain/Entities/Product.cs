using CM_Task.Domain.Exceptions;

namespace CM_Task.Domain.Entities;

public class Product : Entity
{
    private Product()
    {
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }


    public static Product Create(string name, string description, decimal price, int stock)
    {
        return new Product
        {
            Name = name,
            Description = description,
            Price = price,
            Stock = stock
        };
    }

    public void DeductStock(int quantity)
    {
        if (quantity > Stock)
        {
            throw new InsufficientStockException(Id, quantity, Stock);
        }

        Stock -= quantity;
    }
}