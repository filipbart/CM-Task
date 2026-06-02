using CM_Task.Application.Abstractions;
using CM_Task.Application.Products.Commands.CreateProduct;
using CM_Task.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CM_Task.Application.Tests.Products;

public sealed class CreateProductHandlerWriteTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private CreateProductHandler CreateHandler() => new(_productRepository, _unitOfWork);

    [Fact]
    public async Task Handle_PersistsProduct_WithProvidedValues()
    {
        Product? captured = null;
        _productRepository
            .When(r => r.AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>()))
            .Do(ci => captured = ci.Arg<Product>());

        var command = new CreateProductCommand("Coffee Mug", "A nice mug", 29.99m, 100);

        var id = await CreateHandler().Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Name.Should().Be("Coffee Mug");
        captured.Description.Should().Be("A nice mug");
        captured.Price.Should().Be(29.99m);
        captured.Stock.Should().Be(100);
        id.Should().Be(captured.Id);
    }

    [Fact]
    public async Task Handle_AddsProduct_AndSavesChanges_Once()
    {
        var command = new CreateProductCommand("Coffee Mug", "A nice mug", 29.99m, 100);

        await CreateHandler().Handle(command, CancellationToken.None);

        await _productRepository.Received(1).AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
