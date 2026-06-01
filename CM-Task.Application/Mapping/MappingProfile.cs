using AutoMapper;
using CM_Task.Application.Products.Queries.GetProducts;
using CM_Task.Domain.Entities;

namespace CM_Task.Application.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
    }
}