// ProductService.Application/Features/Products/Queries/GetProductById/GetProductByIdQueryHandler.cs
using AutoMapper; // IMapper için
using MediatR;
using ProductService.Application.Features.Products.Dtos; // ProductDto için
using ProductService.Domain.Repositories;             // IProductRepository için
// using ProductService.Application.Exceptions;       // NotFoundException (ileride eklenebilir)

namespace ProductService.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository ?? throw new System.ArgumentNullException(nameof(productRepository));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (product == null)
            {
                return null; // Veya throw new NotFoundException("Product", request.Id);
            }

            return _mapper.Map<ProductDto>(product);
        }
    }
}