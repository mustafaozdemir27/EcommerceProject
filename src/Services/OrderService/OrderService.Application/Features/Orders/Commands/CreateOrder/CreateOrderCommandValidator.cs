// OrderService.Application/Features/Orders/Commands/CreateOrder/CreateOrderCommandValidator.cs
using FluentValidation;
using OrderService.Application.Features.Orders.Dtos;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(p => p.CustomerId)
                .NotEmpty().WithMessage("{PropertyName} alanı zorunludur.")
                .NotEqual(Guid.Empty).WithMessage("{PropertyName} boş bir GUID olamaz.");

            RuleFor(p => p.ShippingAddressStreet)
                .NotEmpty().WithMessage("Gönderim adresi sokak bilgisi zorunludur.")
                .MaximumLength(200).WithMessage("Sokak bilgisi en fazla 200 karakter olmalıdır.");

            RuleFor(p => p.ShippingAddressCity)
                .NotEmpty().WithMessage("Gönderim adresi şehir bilgisi zorunludur.")
                .MaximumLength(100).WithMessage("Şehir bilgisi en fazla 100 karakter olmalıdır.");

            RuleFor(p => p.ShippingAddressCountry)
                .NotEmpty().WithMessage("Gönderim adresi ülke bilgisi zorunludur.")
                .MaximumLength(100).WithMessage("Ülke bilgisi en fazla 100 karakter olmalıdır.");

            RuleFor(p => p.ShippingAddressZipCode)
                .NotEmpty().WithMessage("Gönderim adresi posta kodu zorunludur.")
                .MaximumLength(20).WithMessage("Posta kodu en fazla 20 karakter olmalıdır.");

            RuleFor(p => p.OrderItems)
                .NotEmpty().WithMessage("Sipariş en az bir ürün içermelidir.")
                .Must(items => items != null && items.Any()).WithMessage("Sipariş en az bir ürün içermelidir."); // Ekstra kontrol

            // Her bir sipariş kalemi (OrderItemDto) için ayrı kurallar tanımla
            RuleForEach(p => p.OrderItems).SetValidator(new OrderItemDtoValidator());
        }
    }

    // OrderItemDto için ayrı bir validator sınıfı
    // Bu sınıfı aynı dosyada veya ayrı bir dosyada (örn: OrderItemDtoValidator.cs) tanımlayabilirsiniz.
    // Şimdilik aynı dosyada tutalım.
    public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
    {
        public OrderItemDtoValidator()
        {
            RuleFor(p => p.ProductId)
                .NotEmpty().WithMessage("{PropertyName} alanı zorunludur.")
                .NotEqual(Guid.Empty).WithMessage("{PropertyName} boş bir GUID olamaz.");

            RuleFor(p => p.Quantity)
                .GreaterThan(0).WithMessage("{PropertyName} 0'dan büyük olmalıdır.");
        }
    }
}