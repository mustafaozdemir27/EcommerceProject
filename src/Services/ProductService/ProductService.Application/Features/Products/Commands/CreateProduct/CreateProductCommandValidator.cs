// ProductService.Application/Features/Products/Commands/CreateProduct/CreateProductCommandValidator.cs
using FluentValidation;

namespace ProductService.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} alanı zorunludur.")
                .MaximumLength(200).WithMessage("{PropertyName} en fazla 200 karakter olmalıdır.");

            RuleFor(p => p.Description)
                .MaximumLength(1000).WithMessage("{PropertyName} en fazla 1000 karakter olmalıdır.")
                .When(p => !string.IsNullOrEmpty(p.Description)); // Sadece doluysa bu kuralı uygula

            RuleFor(p => p.PriceAmount)
                .GreaterThan(0).WithMessage("{PropertyName} 0'dan büyük olmalıdır.");

            RuleFor(p => p.Currency)
                .NotEmpty().WithMessage("{PropertyName} alanı zorunludur.")
                .Length(3).WithMessage("{PropertyName} 3 karakter olmalıdır (örn: TRY, USD).");
            // İPUCU: Geçerli para birimi kodlarını bir enum veya liste ile de kontrol edebilirsiniz.
            // .Must(BeAValidCurrency).WithMessage("Lütfen geçerli bir para birimi kodu giriniz.");

            RuleFor(p => p.InitialStock)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} 0 veya daha büyük olmalıdır.");

            RuleFor(p => p.CategoryId)
                .NotEmpty().WithMessage("{PropertyName} alanı zorunludur.")
                .NotEqual(Guid.Empty).WithMessage("{PropertyName} boş bir GUID olamaz.");
        }

        // Örnek özel validasyon metodu (kullanılmıyor ama fikir vermesi için)
        // private bool BeAValidCurrency(string currencyCode)
        // {
        //     // Burada geçerli para birimi kodlarının bir listesiyle kontrol yapılabilir.
        //     // Örneğin: return new List<string> { "TRY", "USD", "EUR" }.Contains(currencyCode.ToUpper());
        //     return true; // Şimdilik her zaman geçerli say
        // }
    }
}