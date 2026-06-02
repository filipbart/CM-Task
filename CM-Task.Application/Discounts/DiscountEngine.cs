using CM_Task.Application.Abstractions;

namespace CM_Task.Application.Discounts;

public sealed class DiscountEngine(IEnumerable<IDiscountRule> rules) : IDiscountEngine
{
    public async Task<DiscountResult?> GetBestDiscount(DiscountContext ctx, CancellationToken ct = default)
    {
        var result = await Task.WhenAll(rules.Select(r => r.Calculate(ctx, ct)));

        return result.Where(r => r is not null).MaxBy(r => r!.Amount);
    }
}