using CM_Task.Application.Discounts;

namespace CM_Task.Application.Abstractions;

public interface IDiscountEngine
{
    Task<DiscountResult?> GetBestDiscount(DiscountContext ctx, CancellationToken ct = default);
}