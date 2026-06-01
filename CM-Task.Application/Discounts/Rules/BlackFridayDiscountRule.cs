using CM_Task.Application.Abstractions;

namespace CM_Task.Application.Discounts.Rules;

public sealed class BlackFridayDiscountRule : IDiscountRule
{
    public Task<DiscountResult?> Calculate(DiscountContext ctx, CancellationToken ct = default)
    {
        if (!IsBlackFriday(ctx.Date)) return Task.FromResult<DiscountResult?>(null);
        var orderTotal = ctx.Lines.Sum(l => l.LineTotal);
        return Task.FromResult<DiscountResult?>(new DiscountResult("Black Friday 25%", orderTotal * 0.25m));
    }

    private static bool IsBlackFriday(DateOnly date)
    {
        var lastDayOfNovember = new DateOnly(date.Year, 11, 30);
        var daysToSubtract = ((int)lastDayOfNovember.DayOfWeek - (int)DayOfWeek.Friday + 7) % 7;
        var blackFriday = lastDayOfNovember.AddDays(-daysToSubtract);
        return date == blackFriday;
    }
}