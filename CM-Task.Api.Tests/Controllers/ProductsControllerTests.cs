using System.Net;
using System.Net.Http.Json;
using CM_Task.Application.Products.Queries.GetProducts;
using FluentAssertions;
using Xunit;

namespace CM_Task.Api.Tests.Controllers;

public sealed class ProductsControllerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ReturnsOk_WithEmptyList()
    {
        var response = await _client.GetAsync("/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content
            .ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_Returns201_WithValidPayload()
    {
        var payload = new
        {
            name = "Coffee Mug",
            description = "A nice mug",
            price = 29.99,
            stock = 100
        };

        var response = await _client.PostAsJsonAsync("/products", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("", "Description", 10, 5)] // brak name
    [InlineData("Name", "", 10, 5)] // brak description
    [InlineData("Name", "Description", 0, 5)] // price = 0
    [InlineData("Name", "Description", -1, 5)] // price ujemne
    [InlineData("Name", "Description", 10, -1)] // stock ujemny
    public async Task Create_Returns400_WithInvalidPayload(
        string name, string description, decimal price, int stock)
    {
        var payload = new { name, description, price, stock };

        var response = await _client.PostAsJsonAsync("/products", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_Returns400_WhenNameExceedsMaxLength()
    {
        var payload = new
        {
            name = new string('x', 51),
            description = "Valid",
            price = 10m,
            stock = 5
        };

        var response = await _client.PostAsJsonAsync("/products", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}