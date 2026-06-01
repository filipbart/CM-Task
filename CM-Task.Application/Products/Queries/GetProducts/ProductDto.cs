namespace CM_Task.Application.Products.Queries.GetProducts;

public sealed record ProductDto(Guid Id, string Name, string Description, decimal Price, int Stock);