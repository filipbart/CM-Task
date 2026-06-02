using AutoMapper;
using CM_Task.Application.Abstractions;
using MediatR;

namespace CM_Task.Application.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var result = await productRepository.GetAllAsync(ct);
        return mapper.Map<IReadOnlyList<ProductDto>>(result);
    }
}