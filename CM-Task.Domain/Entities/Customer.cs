using CM_Task.Domain.Enums;

namespace CM_Task.Domain.Entities;

public sealed class Customer: Entity
{
    private Customer()
    {
    }
    
    public string Name { get; private set; }
    public Region Region { get; private set; }


    public static Customer Create(string name, Region region) => new()
    {
        Name = name,
        Region = region
    };
}