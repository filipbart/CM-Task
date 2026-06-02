using CM_Task.Application.Products.Commands.CreateProduct;
using FluentAssertions;
using Xunit;

namespace CM_Task.Application.Tests.Products;

public sealed class CreateProductValidatorTests
{
    private readonly CreateProductValidator _sut = new();

    [Fact]
    public void Validate_Passes_ForValidCommand()
    {
        var command = new CreateProductCommand("Coffee Mug", "A nice mug", 29.99m, 100);

        var result = _sut.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_Fails_WhenNameMissing(string? name)
    {
        var command = new CreateProductCommand(name!, "desc", 10m, 5);

        var result = _sut.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Fact]
    public void Validate_Fails_WhenNameExceeds50Chars()
    {
        var command = new CreateProductCommand(new string('x', 51), "desc", 10m, 5);

        var result = _sut.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Fact]
    public void Validate_Fails_WhenDescriptionExceeds50Chars()
    {
        var command = new CreateProductCommand("Name", new string('x', 51), 10m, 5);

        var result = _sut.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Description));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_Fails_WhenPriceNotPositive(decimal price)
    {
        var command = new CreateProductCommand("Name", "desc", price, 5);

        var result = _sut.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Price));
    }

    [Fact]
    public void Validate_Fails_WhenStockNegative()
    {
        var command = new CreateProductCommand("Name", "desc", 10m, -1);

        var result = _sut.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Stock));
    }
}
