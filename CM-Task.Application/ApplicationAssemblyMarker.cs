using System.Reflection;

namespace CM_Task.Application;

public sealed class ApplicationAssemblyMarker
{
    public static readonly Assembly Assembly = typeof(ApplicationAssemblyMarker).Assembly;
}