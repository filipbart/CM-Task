using System.ComponentModel.DataAnnotations;

namespace CM_Task.Domain.Entities;

public abstract class Entity
{
    [Key] public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    protected Entity()
    {
        CreatedAt = DateTime.UtcNow;
    }
}