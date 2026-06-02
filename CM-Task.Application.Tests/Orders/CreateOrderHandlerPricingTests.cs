using CM_Task.Application.Abstractions;
using CM_Task.Application.Discounts;
using CM_Task.Application.Orders.Commands.CreateOrder;
using CM_Task.Domain.Entities;
using CM_Task.TestsCore.Builders;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CM_Task.Application.Tests.Orders;

public sealed class CreateOrderHandlerPricingTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly ICustomerRepository _customerRepository = Substitute.For<ICustomerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IDiscountEngine _discountEngine = Substitute.For<IDiscountEngine>();
    private readonly IClock _clock = Substitute.For<IClock>();

    private CreateOrderHandler CreateHandler() => new(
        _productRepository, _orderRepository, _customerRepository, _unitOfWork, _discountEngine, _clock);

    private void Arrange(Customer customer, Product product, DiscountResult? discount)
    {
        _clock.Today.Returns(DiscountContextMother.RegularDay);
        _customerRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        _productRepository.GetByIdsAsync(Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<Guid, Product> { [product.Id] = product });
        _discountEngine.GetBestDiscount(Arg.Any<DiscountContext>(), Arg.Any<CancellationToken>())
            .Returns(discount);
    }

    private async Task<Order> CaptureOrderAsync(CreateOrderCommand command)
    {
        Order? captured = null;
        _orderRepository
            .When(r => r.AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>()))
            .Do(ci => captured = ci.Arg<Order>());

        await CreateHandler().Handle(command, CancellationToken.None);

        captured.Should().NotBeNull();
        return captured!;
    }

    [Fact]
    public async Task Handle_AppliesStandardPricing_ForUsCustomer()
    {
        var customer = CustomerMother.Usa();
        var product = ProductMother.WithPriceAndStock(100m, 10);
        Arrange(customer, product, discount: null);

        var order = await CaptureOrderAsync(new CreateOrderCommand(customer.Id, [new OrderLineRequest(product.Id, 1)]));

        order.Lines[0].UnitPrice.Should().Be(100m);
        order.TotalValue.Should().Be(100m);
    }

    [Fact]
    public async Task Handle_IncreasesPriceBy5Percent_ForAsiaCustomer()
    {
        var customer = CustomerMother.Asia();
        var product = ProductMother.WithPriceAndStock(100m, 10);
        Arrange(customer, product, discount: null);

        var order = await CaptureOrderAsync(new CreateOrderCommand(customer.Id, [new OrderLineRequest(product.Id, 2)]));

        order.Lines[0].UnitPrice.Should().Be(105m);
        order.TotalValue.Should().Be(210m);
    }

    [Fact]
    public async Task Handle_SubtractsDiscount_FromTotal()
    {
        var customer = CustomerMother.Usa();
        var product = ProductMother.WithPriceAndStock(100m, 10);
        Arrange(customer, product, new DiscountResult("Volume 10%", 20m));

        var order = await CaptureOrderAsync(new CreateOrderCommand(customer.Id, [new OrderLineRequest(product.Id, 2)]));
        
        order.TotalValue.Should().Be(180m);
        order.AppliedDiscountName.Should().Be("Volume 10%");
    }

    [Fact]
    public async Task Handle_LeavesDiscountNameNull_WhenNoDiscount()
    {
        var customer = CustomerMother.Usa();
        var product = ProductMother.WithPriceAndStock(100m, 10);
        Arrange(customer, product, discount: null);

        var order = await CaptureOrderAsync(new CreateOrderCommand(customer.Id, [new OrderLineRequest(product.Id, 1)]));

        order.AppliedDiscountName.Should().BeNull();
    }
}
