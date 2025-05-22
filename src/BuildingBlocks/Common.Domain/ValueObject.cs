// Common.Domain/ValueObject.cs
namespace Common.Domain
{
    public abstract class ValueObject
    {
        protected static bool EqualOperator(ValueObject? left, ValueObject? right)
        {
            // XOR (^) operatörü: biri null diğeri değilse true döner (yani eşit değiller)
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            // Her ikisi de null ise veya left.Equals(right) ise true döner
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        protected static bool NotEqualOperator(ValueObject? left, ValueObject? right)
        {
            return !EqualOperator(left, right);
        }

        // Türetilmiş sınıflar, eşitlik karşılaştırmasında kullanılacak bileşenleri bu metot aracılığıyla sağlamalıdır.
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;
            // İki ValueObject'in eşitlik bileşenlerini sırayla karşılaştırır.
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            // Eşitlik bileşenlerinin hash kodlarını birleştirerek genel bir hash kodu oluşturur.
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0) // Null ise 0 al, değilse hash kodunu al
                .Aggregate((x, y) => x ^ y); // Hash kodlarını XOR ile birleştir
        }

        public static bool operator ==(ValueObject? one, ValueObject? two)
        {
            return EqualOperator(one, two);
        }

        public static bool operator !=(ValueObject? one, ValueObject? two)
        {
            return NotEqualOperator(one, two);
        }
    }
}