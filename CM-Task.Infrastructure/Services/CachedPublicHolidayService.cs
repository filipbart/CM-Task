using CM_Task.Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace CM_Task.Infrastructure.Services;

public sealed class CachedPublicHolidayService(NagerPublicHolidayService inner, IMemoryCache cache)
    : IPublicHolidayService
{
    public async Task<bool> IsPublicHolidayAsync(
        DateOnly date, string countryCode, CancellationToken ct = default)
    {
        var key = $"holidays:{countryCode}:{date.Year}";

        var holidays = await cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
            return await inner.GetHolidaysForYearAsync(date.Year, countryCode, ct);
        });

        return holidays?.Contains(date) ?? false;
    }
}