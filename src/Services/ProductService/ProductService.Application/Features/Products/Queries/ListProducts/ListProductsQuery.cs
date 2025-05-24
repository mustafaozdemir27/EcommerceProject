// ProductService.Application/Features/Products/Queries/ListProducts/ListProductsQuery.cs
using MediatR;
using System.Collections.Generic;
using ProductService.Application.Features.Products.Dtos;

namespace ProductService.Application.Features.Products.Queries.ListProducts
{
    public class ListProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
        // Şimdilik parametresiz. İleride filtreleme, sıralama, sayfalama parametreleri eklenebilir.
    }
}