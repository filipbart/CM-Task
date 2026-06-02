using CM_Task.Domain.Entities;

namespace CM_Task.Application.Discounts;

public sealed record DiscountContext(IReadOnlyList<OrderLine> Lines, Customer Customer, DateOnly Date);