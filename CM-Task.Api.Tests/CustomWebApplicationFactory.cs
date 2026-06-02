using Autofac;
using CM_Task.Application.Abstractions;
using CM_Task.Infrastructure.Persistence;
using CM_Task.TestsCore.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace CM_Task.Api.Tests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public DateOnly TestDate { get; set; } = DiscountContextMother.RegularDay;

    public CustomWebApplicationFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureContainer<ContainerBuilder>(container =>
        {
            container.Register(_ =>
                {
                    var options = new DbContextOptionsBuilder<AppDbContext>()
                        .UseSqlite(_connection)
                        .Options;
                    return new AppDbContext(options);
                })
                .As<DbContext>()
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();


            container.Register<IClock>(_ => new TestClock(() => TestDate))
                .InstancePerLifetimeScope();

            container.RegisterInstance(new FakePublicHolidayService(isHoliday: false))
                .As<IPublicHolidayService>()
                .SingleInstance();
        });

        return base.CreateHost(builder);
    }

    public HttpClient CreateClientWithDate(DateOnly date)
    {
        TestDate = date;
        return CreateClient();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _connection.Dispose();
    }
}

public sealed class TestClock(Func<DateOnly> dateProvider) : IClock
{
    public DateOnly Today => dateProvider();
}

public sealed class FakePublicHolidayService(bool isHoliday) : IPublicHolidayService
{
    public Task<bool> IsPublicHolidayAsync(
        DateOnly date, string countryCode, CancellationToken ct = default)
        => Task.FromResult(isHoliday);
}