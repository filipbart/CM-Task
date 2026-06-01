using Autofac;
using Autofac.Extensions.DependencyInjection;
using CM_Task.Api.Middleware;
using CM_Task.Application;
using CM_Task.Application.Mapping;
using FluentValidation;
using CM_Task.Infrastructure.Modules;
using CM_Task.Infrastructure.Persistence;
using CM_Task.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? throw new NullReferenceException("Database connection string is not set.");

    container.RegisterModule(new DatabaseModule(connectionString));
    container.RegisterModule<InfrastructureModule>();
    container.RegisterModule<ApplicationModule>();
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(ApplicationAssemblyMarker.Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(ApplicationModule).Assembly);

builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseRateLimiter();

app.Run();