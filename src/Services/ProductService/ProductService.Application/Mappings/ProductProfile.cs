// ProductService.Application/Mappings/ProductProfile.cs
using AutoMapper;
using ProductService.Application.Features.Products.Dtos;       // ProductDto için
using ProductService.Domain;                                    // Product entity için

namespace ProductService.Application.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Product entity'sinden ProductDto'ya mapping
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.PriceAmount, opt => opt.MapFrom(src => src.CurrentPrice.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.CurrentPrice.Currency));

            // CreateProductCommand'dan Product entity'sine mapping (Opsiyonel)
            // Şu anki CreateProductCommandHandler'ımızda Product'ı manuel oluşturuyoruz,
            // bu yüzden bu mapping'e doğrudan ihtiyacımız yok ama ileride gerekebilir.
            // CreateMap<CreateProductCommand, Product>()
            //    .ConstructUsing(src => new Product(
            //        Guid.NewGuid(),
            //        src.Name,
            //        src.Description,
            //        new Price(src.PriceAmount, src.Currency), // Price VO'sunu oluştur
            //        src.InitialStock,
            //        src.CategoryId
            //    )); 
            // Yukarıdaki ConstructUsing karmaşık gelebilir, alternatif olarak:
            // CreateMap<CreateProductCommand, Product>()
            //    .ForMember(dest => dest.CurrentPrice, opt => opt.MapFrom(src => new Price(src.PriceAmount, src.Currency)))
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())) // Id'yi burada oluşturmak yerine handler'da oluşturmak daha iyi olabilir.
            //    .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(src => DateTime.UtcNow));
            // Şimdilik CreateProductCommand -> Product mapping'ini eklemiyoruz, çünkü handler'da manuel yapıyoruz.
        }
    }
}