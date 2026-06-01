using System.Net.Http.Json;
using CM_Task.Application.Abstractions;

namespace CM_Task.Infrastructure.Services;

public sealed class NagerPublicHolidayService(HttpClient httpClient) : IPublicHolidayService
{
    public async Task<bool> IsPublicHolidayAsync(
        DateOnly date, string countryCode, CancellationToken ct = default)
    {
        var holidays = await GetHolidaysForYearAsync(date.Year, countryCode, ct);
        return holidays.Contains(date);
    }

    public async Task<HashSet<DateOnly>> GetHolidaysForYearAsync(
        int year, string countryCode, CancellationToken ct = default)
    {
        var url = $"api/v3/PublicHolidays/{year}/{countryCode}";

        var holidays = await httpClient
            .GetFromJsonAsync<NagerHolidayDto[]>(url, ct);

        return holidays?
                   .Select(h => DateOnly.FromDateTime(h.Date))
                   .ToHashSet()
               ?? [];
    }
}

file sealed class NagerHolidayDto
{
    public DateTime Date { get; init; }
}