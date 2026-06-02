using CM_Task.Application.Abstractions;
using CM_Task.Application.Discounts;
using CM_Task.Application.Orders.Commands.CreateOrder;
using CM_Task.Domain.Entities;
using CM_Task.Domain.Exceptions;
using CM_Task.TestsCore.Builders;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CM_Task.Application.Tests.Orders;

public sealed class CreateOrderHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly ICustomerRepository _customerRepository = Substitute.For<ICustomerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IDiscountEngine _discountEngine = Substitute.For<IDiscountEngine>();
    private readonly IClock _clock = Substitute.For<IClock>();

    private CreateOrderHandler CreateOrderHandler() => new(_productRepository, _orderRepository, _customerRepository,
        _unitOfWork, _discountEngine, _clock);

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenCustomerNotFound()
    {
        _customerRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Customer?)null);

        var command = new CreateOrderCommand(Guid.NewGuid(), []);
        var handler = CreateOrderHandler();

        await handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_throws_NotFoundException_WhenProductNotFound()
    {
        var customer = CustomerMother.Usa();
        _customerRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        _productRepository.GetByIdsAsync(Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, Product>());

        var command = new CreateOrderCommand(customer.Id,
            [new OrderLineRequest(Guid.NewGuid(), 1)]);
        var handler = CreateOrderHandler();

        await handler.Invoking(s => s.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ThrowsInsufficientStockException_WhenNotEnoughStock()
    {
        var customer = CustomerMother.Usa();
        var product = ProductMother.WithPriceAndStock(100m, 2);

        _customerRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        _productRepository.GetByIdsAsync(Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, Product> { [product.Id] = product });

        var command = new CreateOrderCommand(customer.Id,
            [new OrderLineRequest(product.Id, 5)]);
        var handler = CreateOrderHandler();

        await handler.Invoking(s => s.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InsufficientStockException>();
    }

    [Fact]
    public async Task Handle_AppliesLocationMultiplier_ForEuropeanCustomer()
    {
        var customer = CustomerMother.Europe();
        var product = ProductMother.WithPriceAndStock(100m, 10);
        _clock.Today.Returns(DiscountContextMother.RegularDay);

        _customerRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        _productRepository.GetByIdsAsync(Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, Product> { [product.Id] = product });
        _discountEngine.GetBestDiscount(Arg.Any<DiscountContext>(), Arg.Any<CancellationToken>())
            .Returns((DiscountResult?)null);

        var command = new CreateOrderCommand(customer.Id, [new OrderLineRequest(product.Id, 2)]);
        var handler = CreateOrderHandler();

        await handler.Handle(command, CancellationToken.None);

        await _orderRepository.Received(1).AddAsync(Arg.Is<Order>(o =>
            o.CustomerId == customer.Id &&
            o.Lines.Count == 1 &&
            o.Lines[0].ProductId == product.Id &&
            o.Lines[0].Quantity == 2 &&
            o.Lines[0].UnitPrice == 100m * 1.15m
        ), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeductsStock_AfterOrderCreated()
    {
        var customer = CustomerMother.Usa();
        var product = ProductMother.WithPriceAndStock(100m, 10);
        _clock.Today.Returns(DiscountContextMother.RegularDay);

        _customerRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        _productRepository.GetByIdsAsync(Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, Product> { [product.Id] = product });
        _discountEngine.GetBestDiscount(Arg.Any<DiscountContext>(), Arg.Any<CancellationToken>())
            .Returns((DiscountResult?)null);

        var command = new CreateOrderCommand(customer.Id, [new OrderLineRequest(product.Id, 2)]);
        var handler = CreateOrderHandler();

        await handler.Handle(command, CancellationToken.None);

        product.Stock.Should().Be(8);
    }
}