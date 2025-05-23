// UserService.Infrastructure/Security/BCryptPasswordHasherService.cs
using UserService.Application.Contracts.Security; // IPasswordHasherService için
// BCrypt.Net.BCrypt sınıfını kullanmak için using ifadesi:
// Eğer BCrypt.Net-Next paketini UserService.Application'a eklediyseniz
// ve UserService.Infrastructure, UserService.Application'a referans veriyorsa
// bu using'i doğrudan yazabilirsiniz.
// Aksi halde, BCrypt.Net-Next paketini UserService.Infrastructure projesine de ekleyin.
// using BCrypt.Net; // Bu şekilde veya doğrudan BCrypt.Net.BCrypt. metodunu çağırın

namespace UserService.Infrastructure.Security
{
    public class BCryptPasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new System.ArgumentException("Şifre boş olamaz.", nameof(password));
            }
            // BCrypt.Net.BCrypt.HashPassword metodu zaten salt üretip dahil eder.
            return global::BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(providedPassword))
            {
                return false;
            }
            // BCrypt.Net.BCrypt.Verify metodu, hash'lenmiş şifre içindeki salt'ı kullanarak karşılaştırma yapar.
            return global::BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
    }
}