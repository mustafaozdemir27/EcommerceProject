// UserService.Application/Features/Users/Commands/UpdateUserProfile/UpdateUserProfileCommandValidator.cs
using FluentValidation;

namespace UserService.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            // Id alanı genellikle route'tan gelir ve API seviyesinde varlığı kontrol edilir (örneğin Guid formatı).
            // Komut içinde de boş olmaması gerektiğini belirtebiliriz.
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} alanı zorunludur.");

            // FirstName alanı için kurallar:
            // Sadece null değilse ve boş değilse uzunluk kontrolü yap.
            // Bu, alanın opsiyonel olarak güncellenmesine izin verir.
            // Eğer alan her zaman zorunluysa, .NotEmpty() ile başlardık.
            When(p => !string.IsNullOrWhiteSpace(p.FirstName), () => {
                RuleFor(p => p.FirstName)
                    .MinimumLength(2).WithMessage("Ad en az 2 karakter olmalıdır.")
                    .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olmalıdır.");
            });

            // LastName alanı için kurallar:
            When(p => !string.IsNullOrWhiteSpace(p.LastName), () => {
                RuleFor(p => p.LastName)
                    .MinimumLength(2).WithMessage("Soyad en az 2 karakter olmalıdır.")
                    .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olmalıdır.");
            });

            // Not: Eğer FirstName veya LastName'den en az birinin girilmesi zorunluysa,
            // daha karmaşık bir kural yazılabilir:
            // RuleFor(p => p)
            //     .Must(p => !string.IsNullOrWhiteSpace(p.FirstName) || !string.IsNullOrWhiteSpace(p.LastName))
            //     .WithMessage("Ad veya Soyad alanlarından en az biri doldurulmalıdır.");
            // Şimdilik, her iki alan da bağımsız olarak güncellenebilir veya boş bırakılabilir (eğer null gönderilirse).
            // Ancak API'den boş string ("") gönderilirse, When koşulu çalışır ve MinimumLength hatası verebilir.
            // Bu yüzden API'den null göndermek veya hiç göndermemek daha doğru olur opsiyonel alanlar için.
            // Ya da NotEmpty().When(p => p.FirstName != null) gibi kurallar eklenebilir.

            // Şimdilik, eğer bir değer verilmişse, çok kısa veya çok uzun olmamasını sağlıyoruz.
            // API'den gelen JSON'da alan hiç yoksa veya değeri null ise bu kurallar tetiklenmez.
            // Eğer alan var ama değeri "" (boş string) ise, When koşulu true olur ve MinimumLength devreye girer.
            // Bu davranış istenmiyorsa, When(p => p.FirstName != null) kullanılıp ardından
            // RuleFor(p => p.FirstName).NotEmpty().MinimumLength... gibi devam edilebilir.

            // Basitlik adına, şimdilik eğer bir değer gönderildiyse (null veya whitespace değilse) geçerli uzunlukta olmasını bekleyelim.
            // Handler'ımızda null gelen alanları güncellemeyeceğiz.
        }
    }
}