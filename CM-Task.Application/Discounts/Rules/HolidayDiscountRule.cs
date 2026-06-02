using CM_Task.Application.Abstractions;

namespace CM_Task.Application.Discounts.Rules;

public sealed class HolidayDiscountRule(IPublicHolidayService holidayService) : IDiscountRule
{
    public async Task<DiscountResult?> Calculate(DiscountContext ctx, CancellationToken ct = default)
    {
        var isHoliday = await holidayService.IsPublicHolidayAsync(ctx.Date, "PL", ct);
        if (!isHoliday) return null;

        var mostExpensiveLine = ctx.Lines.MaxBy(l => l.UnitPrice);
        if (mostExpensiveLine is null) return null;

        var discountAmount = mostExpensiveLine.UnitPrice * mostExpensiveLine.Quantity * 0.15m;

        return new DiscountResult("Holiday 15%", discountAmount);
    }
}