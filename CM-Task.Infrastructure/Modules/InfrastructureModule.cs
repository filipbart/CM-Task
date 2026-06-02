using Autofac;
using CM_Task.Application.Abstractions;
using CM_Task.Infrastructure.Clock;
using CM_Task.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;

namespace CM_Task.Infrastructure.Modules;

public sealed class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<SystemClock>()
            .As<IClock>()
            .SingleInstance();

        builder.Register(_ =>
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri("https://date.nager.at/")
                };
                return client;
            })
            .Named<HttpClient>("nager")
            .SingleInstance();

        builder.Register(ctx =>
                new NagerPublicHolidayService(ctx.ResolveNamed<HttpClient>("nager")))
            .AsSelf()
            .SingleInstance();

        builder.Register(ctx =>
                new CachedPublicHolidayService(
                    ctx.Resolve<NagerPublicHolidayService>(),
                    ctx.Resolve<IMemoryCache>()))
            .As<IPublicHolidayService>()
            .SingleInstance();
    }
}