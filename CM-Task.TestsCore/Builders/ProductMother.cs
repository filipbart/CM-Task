using CM_Task.Domain.Entities;

namespace CM_Task.TestsCore.Builders;

public static class ProductMother
{
    public static Product Default() =>
        Product.Create("Test Product", "Test Description", 100m, 50);

    public static Product WithStock(int stock) =>
        Product.Create("Test Product", "Test Description", 100m, stock);

    public static Product WithPrice(decimal price) =>
        Product.Create("Test Product", "Test Description", price, 50);

    public static Product WithPriceAndStock(decimal price, int stock) =>
        Product.Create("Test Product", "Test Description", price, stock);

    public static Product OutOfStock() =>
        Product.Create("Test Product", "Test Description", 100m, 0);
}