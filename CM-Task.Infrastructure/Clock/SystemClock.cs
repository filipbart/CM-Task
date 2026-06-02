using CM_Task.Application.Abstractions;

namespace CM_Task.Infrastructure.Clock;

public sealed class SystemClock : IClock
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}