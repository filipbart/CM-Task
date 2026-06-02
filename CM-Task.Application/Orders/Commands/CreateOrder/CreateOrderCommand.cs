using FluentValidation;
using MediatR;

namespace CM_Task.Application.Orders.Commands.CreateOrder;

public sealed record OrderLineRequest(Guid ProductId, int Quantity);

public sealed record CreateOrderCommand(Guid CustomerId, IReadOnlyList<OrderLineRequest> Lines) : IRequest<Guid>;

public sealed class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("Order must contain at least one line.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ProductId).NotEmpty();
            line.RuleFor(l => l.Quantity).GreaterThan(0);
        });
    }
}