using System.Net;
using System.Net.Http.Json;
using CM_Task.Domain.Enums;
using CM_Task.TestsCore.Builders;
using CM_Task.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Xunit;

namespace CM_Task.Api.Tests.Controllers;

public sealed class OrdersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public OrdersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private static async Task<Guid> CreateProductAsync(
        HttpClient client, decimal price = 100m, int stock = 50)
    {
        var response = await client.PostAsJsonAsync("/products", new
        {
            name = "Test Product",
            description = "Test Desc",
            price,
            stock
        });
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    [Fact]
    public async Task Create_Returns201_HappyPath()
    {
        var client = _factory.CreateClient();
        var productId = await CreateProductAsync(client);
        var customerId = await GetSeedCustomerIdAsync(Region.Usa);

        var response = await client.PostAsJsonAsync("/orders", new
        {
            customerId,
            lines = new[] { new { productId, quantity = 1 } }
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_Returns409_WhenInsufficientStock()
    {
        var client = _factory.CreateClient();
        var productId = await CreateProductAsync(client, stock: 2);
        var customerId = await GetSeedCustomerIdAsync(Region.Usa);

        var response = await client.PostAsJsonAsync("/orders", new
        {
            customerId,
            lines = new[] { new { productId, quantity = 10 } }
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_Returns404_WhenProductNotFound()
    {
        var client = _factory.CreateClient();
        var customerId = await GetSeedCustomerIdAsync(Region.Usa);

        var response = await client.PostAsJsonAsync("/orders", new
        {
            customerId,
            lines = new[] { new { productId = Guid.NewGuid(), quantity = 1 } }
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_AppliesVolumeDiscount_WhenQuantityAboveThreshold()
    {
        var client = _factory.CreateClient();
        var productId = await CreateProductAsync(client, price: 100m, stock: 100);
        var customerId = await GetSeedCustomerIdAsync(Region.Usa);

        var response = await client.PostAsJsonAsync("/orders", new
        {
            customerId,
            lines = new[] { new { productId, quantity = 10 } }
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_AppliesBlackFridayDiscount_OnBlackFriday()
    {
        var client = _factory.CreateClientWithDate(DiscountContextMother.BlackFriday2025);
        var productId = await CreateProductAsync(client, price: 100m, stock: 10);
        var customerId = await GetSeedCustomerIdAsync(Region.Usa);

        var response = await client.PostAsJsonAsync("/orders", new
        {
            customerId,
            lines = new[] { new { productId, quantity = 1 } }
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_AppliesLocationMultiplier_ForEuropeanCustomer()
    {
        var client = _factory.CreateClient();
        var productId = await CreateProductAsync(client, price: 100m, stock: 10);
        var customerId = await GetSeedCustomerIdAsync(Region.Europe);

        var response = await client.PostAsJsonAsync("/orders", new
        {
            customerId,
            lines = new[] { new { productId, quantity = 1 } }
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private async Task<Guid> GetSeedCustomerIdAsync(
        Region region)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Region == region);
        if (customer is null)
            throw new InvalidOperationException($"Seed customer for region {region} not found");
        return customer.Id;
    }
}