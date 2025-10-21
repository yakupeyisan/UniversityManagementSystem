namespace UniversityMS.Domain.Enums;

/// <summary>
/// Kafeteria Ödeme Yöntemi
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Kafeteria Kartı - Ön yüklü kart (bakiye kullan)
    /// </summary>
    CafeteriaCard = 1,

    /// <summary>
    /// Nakit - Direkt ödeme
    /// </summary>
    Cash = 2,

    /// <summary>
    /// Kredi Kartı - Online ödeme
    /// </summary>
    CreditCard = 3,

    /// <summary>
    /// Banka Transferi - Toplu yükleme (kurum tarafından)
    /// </summary>
    BankTransfer = 4,

    /// <summary>
    /// Mobil Ödeme - Telefon ile ödeme
    /// </summary>
    MobilePayment = 5,

    /// <summary>
    /// QR Kodu - QR kodu tarayarak ödeme
    /// </summary>
    QRCode = 6
}