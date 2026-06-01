using Autofac;
using CM_Task.Application.Abstractions;
using CM_Task.Application.Common.Behaviors;
using CM_Task.Application.Discounts;
using CM_Task.Application.Discounts.Rules;
using MediatR;

namespace CM_Task.Infrastructure.Modules;

public sealed class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(BlackFridayDiscountRule).Assembly)
            .AssignableTo<IDiscountRule>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterGeneric(typeof(ValidationBehavior<,>))
            .As(typeof(IPipelineBehavior<,>))
            .InstancePerLifetimeScope();

        builder.RegisterType<DiscountEngine>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}