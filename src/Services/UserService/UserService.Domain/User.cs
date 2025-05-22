// UserService.Domain/User.cs
using Common.Domain;
using UserService.Domain.Events; // Entity<TId> ve IAggregateRoot için

namespace UserService.Domain
{
    public class User : Entity<Guid>, IAggregateRoot
    {
        // Özellikler (Properties)
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; } // Şifrenin hash'lenmiş hali
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; } // Null olabilir (yeni kullanıcı için)
        public bool IsActive { get; private set; }

        // EF Core'un proxy oluşturması ve materyalizasyon için private/protected parametresiz constructor.
        private User() : base()
        {
            // Bu constructor EF Core tarafından kullanılır.
            // Username, Email vb. null olabilir, bu yüzden null forgiving operatörü (!) kullanıyoruz
            // ya da property'leri nullable yapıp constructor'da atama garantisi vermeliyiz.
            // Şimdilik EF Core'un bu constructor'ı kullanacağını ve property'leri dolduracağını varsayıyoruz.
            Username = null!;
            Email = null!;
            PasswordHash = null!;
            FirstName = null!;
            LastName = null!;
        }

        // Yeni bir kullanıcı oluşturmak için public constructor
        public User(Guid id, string username, string email, string passwordHash, string firstName, string lastName)
            : base(id) // Temel Entity sınıfının constructor'ını çağırıyoruz (ID ataması için)
        {
            // Basit doğrulamalar (daha kapsamlısı FluentValidation ile Application katmanında yapılabilir)
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username), "Username cannot be empty.");
            if (string.IsNullOrWhiteSpace(email)) // TODO: Email format doğrulaması eklenecek
                throw new ArgumentNullException(nameof(email), "Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentNullException(nameof(passwordHash), "Password hash cannot be empty.");
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentNullException(nameof(firstName), "First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentNullException(nameof(lastName), "Last name cannot be empty.");

            Username = username;
            Email = email.ToLowerInvariant(); // E-postayı küçük harfe çevirerek saklamak tutarlılık sağlar
            PasswordHash = passwordHash;     // Şifrenin zaten hash'lenmiş olarak geldiğini varsayıyoruz
            FirstName = firstName;
            LastName = lastName;

            CreatedAtUtc = DateTime.UtcNow;
            IsActive = true; // Yeni kullanıcı varsayılan olarak aktif olsun
                             // (veya e-posta onayı gibi bir süreçle aktifleştirilebilir)

            // Domain event'ini yayınla
            AddDomainEvent(new UserRegisteredDomainEvent(this.Id, this.Email, this.FirstName, this.LastName, this.CreatedAtUtc));
        }

        // Davranışlar (Methods) - Şimdilik birkaç örnek
        public void UpdateProfile(string? newFirstName, string? newLastName) // Parametreleri nullable yap
        {
            bool profileChanged = false;

            // FirstName'i güncelle (eğer yeni değer null değil, boş değil ve mevcut değerden farklıysa)
            if (newFirstName != null && !string.IsNullOrWhiteSpace(newFirstName) && FirstName != newFirstName)
            {
                // Validator'da temel uzunluk kontrolleri yapıldı,
                // burada da ek domain'e özgü validasyonlar yapılabilir.
                FirstName = newFirstName;
                profileChanged = true;
            }

            // LastName'i güncelle (eğer yeni değer null değil, boş değil ve mevcut değerden farklıysa)
            if (newLastName != null && !string.IsNullOrWhiteSpace(newLastName) && LastName != newLastName)
            {
                LastName = newLastName;
                profileChanged = true;
            }

            if (profileChanged)
            {
                UpdatedAtUtc = DateTime.UtcNow;
                AddDomainEvent(new UserProfileUpdatedDomainEvent(this.Id, UpdatedAtUtc.Value));
            }
        }

        public void Deactivate()
        {
            if (!IsActive)
            {
                // Belki bir domain exception fırlatılabilir veya sessizce geçilebilir.
                // throw new UserAlreadyInactiveException(this.Id);
                return;
            }
            IsActive = false;
            UpdatedAtUtc = DateTime.UtcNow;
            // TODO: Kullanıcı deaktive edildiğinde bir domain event'i yayınla
        }

        public void Activate()
        {
            if (IsActive)
            {
                // throw new UserAlreadyActiveException(this.Id);
                return;
            }
            IsActive = true;
            UpdatedAtUtc = DateTime.UtcNow;
            // TODO: Kullanıcı aktive edildiğinde bir domain event'i yayınla
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentNullException(nameof(newPasswordHash), "New password hash cannot be empty.");

            PasswordHash = newPasswordHash;
            UpdatedAtUtc = DateTime.UtcNow;
            // TODO: Şifre değiştiğinde bir domain event'i yayınla
        }
    }
}