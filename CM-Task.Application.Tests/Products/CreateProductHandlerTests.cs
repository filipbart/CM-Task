using AutoMapper;
using CM_Task.Application.Abstractions;
using CM_Task.Application.Products.Queries.GetProducts;
using CM_Task.Domain.Entities;
using CM_Task.TestsCore.Builders;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CM_Task.Application.Tests.Products;

public sealed class CreateProductHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();

    private GetProductsQueryHandler CreateHandler() => new(_productRepository, _mapper);

    [Fact]
    public async Task Handle_ReturnsAllProducts()
    {
        var productList = new List<Product>
        {
            ProductMother.Default(),
            ProductMother.WithPrice(200m)
        };

        _productRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(productList);
        _mapper.Map<IReadOnlyList<ProductDto>>(productList)
            .Returns(new List<ProductDto>
            {
                new(Guid.NewGuid(), "Test Product", "Test Description", 100m, 50),
                new(Guid.NewGuid(), "Test Product", "Test Description", 200m, 50),
            });

        var result = await CreateHandler().Handle(new GetProductsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoProducts()
    {
        _productRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Product>());
        _mapper.Map<IReadOnlyList<ProductDto>>(Arg.Any<List<Product>>())
            .Returns(new List<ProductDto>());

        var result = await CreateHandler().Handle(new GetProductsQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}