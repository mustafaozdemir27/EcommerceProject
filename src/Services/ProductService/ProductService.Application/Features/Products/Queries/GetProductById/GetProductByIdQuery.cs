// ProductService.Application/Features/Products/Queries/GetProductById/GetProductByIdQuery.cs
using MediatR;
using ProductService.Application.Features.Products.Dtos; // ProductDto için

namespace ProductService.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<ProductDto?> // ProductDto? : Ürün bulunamazsa null dönebilir
    {
        public Guid Id { get; }

        public GetProductByIdQuery(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Ürün ID'si boş olamaz.", nameof(id));
            }
            Id = id;
        }
    }
}