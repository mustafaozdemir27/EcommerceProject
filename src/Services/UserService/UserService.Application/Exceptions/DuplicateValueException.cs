// UserService.Application/Exceptions/DuplicateValueException.cs
namespace UserService.Application.Exceptions
{
    public class DuplicateValueException : Exception // System.Exception'dan miras alıyoruz
    {
        public DuplicateValueException(string message)
            : base(message)
        {
        }

        public DuplicateValueException(string entityName, string fieldName, object fieldValue)
            : base($"'{entityName}' varlığı için '{fieldName}' alanındaki '{fieldValue}' değeri zaten mevcut veya benzersizlik kısıtlamasını ihlal ediyor.")
        {
            // Hata mesajını daha sonra programatik olarak ayrıştırmak isterseniz,
            // bu bilgileri public özellikler olarak da saklayabilirsiniz:
            // public string EntityName { get; } = entityName;
            // public string FieldName { get; } = fieldName;
            // public object FieldValue { get; } = fieldValue;
        }

        public DuplicateValueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}