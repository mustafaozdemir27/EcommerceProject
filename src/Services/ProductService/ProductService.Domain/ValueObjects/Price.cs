// ProductService.Domain/ValueObjects/Price.cs
using Common.Domain; // ValueObject temel sınıfı için

namespace ProductService.Domain.ValueObjects
{
    public class Price : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } // Örneğin "TRY", "USD", "EUR"

        // EF Core için özel parametresiz constructor
        private Price() { }

        public Price(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Fiyat miktarı negatif olamaz.");
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentNullException(nameof(currency), "Para birimi boş olamaz.");
            // Para birimi için daha detaylı validasyonlar (örn: geçerli ISO kodları) eklenebilir.

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString() => $"{Amount} {Currency}";
    }
}