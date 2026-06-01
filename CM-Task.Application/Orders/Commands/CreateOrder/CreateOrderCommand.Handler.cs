using CM_Task.Application.Abstractions;
using CM_Task.Application.Discounts;
using CM_Task.Domain.Entities;
using CM_Task.Domain.Enums;
using CM_Task.Domain.Exceptions;
using MediatR;

namespace CM_Task.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderHandler(
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    DiscountEngine discountEngine,
    IClock clock) : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsync(command.CustomerId, ct)
                       ?? throw new NotFoundException(nameof(Customer), command.CustomerId);

        var productIds = command.Lines.Select(l => l.ProductId).Distinct().ToList();
        var productMap = await productRepository.GetByIdsAsync(productIds, ct);

        foreach (var line in command.Lines)
        {
            if (!productMap.TryGetValue(line.ProductId, out var product))
                throw new NotFoundException(nameof(Product), line.ProductId);

            product.DeductStock(line.Quantity);
        }

        var order = Order.Create(customer, DateTime.UtcNow);

        var multiplier = GetLocationMultiplier(customer.Region);

        foreach (var line in command.Lines)
        {
            var product = productMap[line.ProductId];
            var unitPrice = product.Price * multiplier;

            order.AddLine(OrderLine.Create(
                order.Id,
                product.Id,
                product.Name,
                line.Quantity,
                unitPrice));
        }

        var ctx = new DiscountContext(order.Lines, customer, clock.Today);
        var discount = await discountEngine.GetBestDiscount(ctx, ct);

        var total = order.Lines.Sum(l => l.LineTotal);
        var finalValue = total - (discount?.Amount ?? 0m);

        order.ApplyPricing(finalValue, discount?.Name);

        await orderRepository.AddAsync(order, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return order.Id;
    }

    private static decimal GetLocationMultiplier(Region region) => region switch
    {
        Region.Europe => 1.15m,
        Region.Asia => 1.05m,
        Region.Usa => 1.00m,
        _ => throw new ArgumentOutOfRangeException(nameof(region))
    };
}