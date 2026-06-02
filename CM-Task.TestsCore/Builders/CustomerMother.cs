using CM_Task.Domain.Entities;
using CM_Task.Domain.Enums;

namespace CM_Task.TestsCore.Builders;

public static class CustomerMother
{
    public static Customer Usa() => Customer.Create("Alice Johnson", Region.Usa);
    public static Customer Europe() => Customer.Create("Jan Kowalski", Region.Europe);
    public static Customer Asia() => Customer.Create("Yuki Tanaka", Region.Asia);

    public static Customer WithRegion(Region region) => region switch
    {
        Region.Usa => Usa(),
        Region.Europe => Europe(),
        Region.Asia => Asia(),
        _ => throw new ArgumentOutOfRangeException(nameof(region))
    };
}