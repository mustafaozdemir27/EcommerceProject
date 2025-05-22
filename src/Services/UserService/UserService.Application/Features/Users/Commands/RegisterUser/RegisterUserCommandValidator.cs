// UserService.Application/Features/Users/Commands/RegisterUser/RegisterUserCommandValidator.cs
using FluentValidation;

namespace UserService.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(p => p.Username)
                .NotEmpty().WithMessage("{PropertyName} alanı zorunludur.")
                .MinimumLength(3).WithMessage("{PropertyName} en az 3 karakter olmalıdır.")
                .MaximumLength(50).WithMessage("{PropertyName} en fazla 50 karakter olmalıdır.");
            // Not: Kullanıcı adı benzersizliği gibi veritabanı kontrolleri
            // IUserRepository inject edilerek burada da yapılabilir (.MustAsync ile),
            // ancak bu kontrolü zaten Handler içinde yapıyoruz.
            // Tercihe göre validasyon katmanında da bu tür kontroller eklenebilir.

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("{PropertyName} alanı zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir {PropertyName} adresi giriniz.");
            // Not: E-posta benzersizliği kontrolü de benzer şekilde burada yapılabilir.

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage("{PropertyName} alanı zorunludur.")
                .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
                // Daha karmaşık şifre kuralları eklenebilir:
                // .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
                // .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
                // .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
                // .Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az bir özel karakter içermelidir.")
                .MaximumLength(100).WithMessage("Şifre en fazla 100 karakter olmalıdır.");

            RuleFor(p => p.FirstName)
                .NotEmpty().WithMessage("{PropertyName} alanı zorunludur.")
                .MaximumLength(50).WithMessage("{PropertyName} en fazla 50 karakter olmalıdır.");

            RuleFor(p => p.LastName)
                .NotEmpty().WithMessage("{PropertyName} alanı zorunludur.")
                .MaximumLength(50).WithMessage("{PropertyName} en fazla 50 karakter olmalıdır.");
        }
    }
}