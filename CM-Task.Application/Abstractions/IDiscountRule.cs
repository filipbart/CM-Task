using CM_Task.Application.Discounts;

namespace CM_Task.Application.Abstractions;

public interface IDiscountRule
{
    Task<DiscountResult?> Calculate(DiscountContext ctx, CancellationToken ct = default);
}