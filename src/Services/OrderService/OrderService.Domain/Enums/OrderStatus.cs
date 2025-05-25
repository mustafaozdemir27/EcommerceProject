namespace OrderService.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,        // Beklemede (Yeni oluşturuldu, ödeme bekleniyor vb.)
        Processing,     // İşleniyor (Ödeme alındı, hazırlık aşamasında)
        Shipped,        // Kargolandı
        Delivered,      // Teslim Edildi
        Cancelled,      // İptal Edildi
        Failed          // Başarısız (Ödeme hatası vb.)
    }
}
