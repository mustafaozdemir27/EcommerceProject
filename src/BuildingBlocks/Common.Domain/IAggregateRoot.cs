namespace Common.Domain
{
    public interface IAggregateRoot
    {
        // Bu bir işaretleyici arayüzdür (marker interface).
        // Şimdilik herhangi bir üye (metot veya özellik) tanımlamasına gerek yoktur.
        // Aggregate Root olan Entity sınıflarımız bu arayüzü implemente edecek.
    }
}
