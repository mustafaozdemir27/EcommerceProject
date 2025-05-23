// UserService.Application/Contracts/Security/IPasswordHasherService.cs
namespace UserService.Application.Contracts.Security
{
    public interface IPasswordHasherService
    {
        /// <summary>
        /// Verilen şifreyi hash'ler.
        /// </summary>
        /// <param name="password">Hash'lenecek düz metin şifre.</param>
        /// <returns>Hash'lenmiş şifre.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verilen düz metin şifreyi, daha önce hash'lenmiş bir şifreyle doğrular.
        /// </summary>
        /// <param name="hashedPassword">Veritabanında saklanan hash'lenmiş şifre.</param>
        /// <param name="providedPassword">Kullanıcının girdiği düz metin şifre.</param>
        /// <returns>Şifreler eşleşiyorsa true, aksi halde false.</returns>
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}