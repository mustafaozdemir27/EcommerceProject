// ProductService.Application/Features/Products/Queries/ListProducts/ListProductsQueryHandler.cs
using AutoMapper;
using MediatR;
using ProductService.Application.Features.Products.Dtos;
using ProductService.Domain.Repositories;

namespace ProductService.Application.Features.Products.Queries.ListProducts
{
    public class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ListProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository ?? throw new System.ArgumentNullException(nameof(productRepository));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ProductDto>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync(cancellationToken);

            // Eğer products null ise veya boşsa, boş bir liste döndür.
            // _mapper.Map bu durumu doğru yönetir (null ise null, boş ise boş koleksiyon).
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}