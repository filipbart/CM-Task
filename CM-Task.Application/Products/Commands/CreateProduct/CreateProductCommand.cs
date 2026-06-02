using CM_Task.Application.Abstractions;
using CM_Task.Domain.Entities;
using FluentValidation;
using MediatR;

namespace CM_Task.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(string Name, string Description, decimal Price, int Stock) : IRequest<Guid>;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateProductHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateProductCommand cmd, CancellationToken ct)
    {
        var product = Product.Create(
            cmd.Name, cmd.Description, cmd.Price, cmd.Stock);

        await productRepository.AddAsync(product, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return product.Id;
    }
}