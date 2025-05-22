// UserService.Application/Exceptions/NotFoundException.cs
namespace UserService.Application.Exceptions
{
    public class NotFoundException : Exception // System.Exception'dan miras alıyoruz
    {
        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string entityName, object key)
            : base($"'{entityName}' tipindeki '{key}' anahtarına sahip varlık bulunamadı.")
        {
            // Hata mesajını daha sonra programatik olarak ayrıştırmak isterseniz,
            // bu bilgileri public özellikler olarak da saklayabilirsiniz:
            // public string EntityName { get; } = entityName;
            // public object Key { get; } = key;
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}