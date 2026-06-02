using CM_Task.Application.Orders.Commands.CreateOrder;
using FluentAssertions;
using Xunit;

namespace CM_Task.Application.Tests.Orders;

public sealed class CreateOrderValidatorTests
{
    private readonly CreateOrderValidator _sut = new();

    [Fact]
    public void Validate_Passes_ForValidCommand()
    {
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            [new OrderLineRequest(Guid.NewGuid(), 1)]);

        var result = _sut.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Fails_WhenCustomerIdEmpty()
    {
        var command = new CreateOrderCommand(
            Guid.Empty,
            [new OrderLineRequest(Guid.NewGuid(), 1)]);

        var result = _sut.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateOrderCommand.CustomerId));
    }

    [Fact]
    public void Validate_Fails_WhenNoLines()
    {
        var command = new CreateOrderCommand(Guid.NewGuid(), []);

        var result = _sut.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateOrderCommand.Lines));
    }

    [Fact]
    public void Validate_Fails_WhenLineProductIdEmpty()
    {
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            [new OrderLineRequest(Guid.Empty, 1)]);

        var result = _sut.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_Fails_WhenLineQuantityNotPositive(int quantity)
    {
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            [new OrderLineRequest(Guid.NewGuid(), quantity)]);

        var result = _sut.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
