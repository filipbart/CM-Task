namespace CM_Task.Application.Abstractions;

public interface IClock
{
    public DateOnly Today { get; }
}