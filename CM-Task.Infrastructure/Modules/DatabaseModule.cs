using Autofac;
using CM_Task.Application.Abstractions;
using CM_Task.Infrastructure.Clock;
using CM_Task.Infrastructure.Persistence;
using CM_Task.Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CM_Task.Infrastructure.Modules;

public sealed class DatabaseModule : Module
{
    private readonly string _connectionString;
    private static bool _loaded;

    public DatabaseModule(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AppDbContext>().AsSelf().InstancePerLifetimeScope();

        if (_loaded) return;
        _loaded = true;

        var dataSourceBuilder = new SqliteConnectionStringBuilder(_connectionString);

        builder.Register(_ =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite(dataSourceBuilder.ToString());

                return new AppDbContext(optionsBuilder.Options);
            })
            .As<DbContext>()
            .AsImplementedInterfaces()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(typeof(IRepository).Assembly, typeof(ProductRepository).Assembly)
            .Where(t => t.IsAssignableTo<IRepository>())
            .AsImplementedInterfaces();

        builder.RegisterType<SystemClock>()
            .As<IClock>()
            .SingleInstance();
    }
}