namespace CM_Task.Application.Abstractions;

public interface IPublicHolidayService
{
    Task<bool> IsPublicHolidayAsync(DateOnly date, string countryCode, CancellationToken ct = default);
}