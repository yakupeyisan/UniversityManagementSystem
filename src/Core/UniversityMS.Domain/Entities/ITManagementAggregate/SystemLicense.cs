using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.ITManagementAggregate;

/// <summary>
/// Sistem Lisansı
/// </summary>
public class SystemLicense : AuditableEntity
{
    public string SoftwareName { get; private set; } = null!;
    public string LicenseKey { get; private set; } = null!;
    public LicenseType Type { get; private set; }
    public DateTime PurchaseDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public int UserCount { get; private set; }
    public decimal Cost { get; private set; }
    public LicenseStatus Status { get; private set; }
    public bool AutoRenewal { get; private set; }

    private SystemLicense() { }

    private SystemLicense(string softwareName, string licenseKey, LicenseType type, DateTime purchaseDate, DateTime expiryDate, int userCount, decimal cost)
    {
        SoftwareName = softwareName;
        LicenseKey = licenseKey;
        Type = type;
        PurchaseDate = purchaseDate;
        ExpiryDate = expiryDate;
        UserCount = userCount;
        Cost = cost;
        Status = LicenseStatus.Active;
        AutoRenewal = false;
    }

    public static SystemLicense Create(string softwareName, string licenseKey, LicenseType type, DateTime purchaseDate, DateTime expiryDate, int userCount, decimal cost)
    {
        if (string.IsNullOrWhiteSpace(softwareName) || string.IsNullOrWhiteSpace(licenseKey))
            throw new DomainException("Yazılım adı ve lisans anahtarı boş olamaz.");
        if (expiryDate <= purchaseDate)
            throw new DomainException("Son kullanma tarihi satın alma tarihinden sonra olmalıdır.");
        return new SystemLicense(softwareName, licenseKey, type, purchaseDate, expiryDate, userCount, cost);
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiryDate;
    public bool IsExpiringSoon(int daysWarning = 30) => !IsExpired() && ExpiryDate <= DateTime.UtcNow.AddDays(daysWarning);
    public void Renew(DateTime newExpiryDate)
    {
        if (newExpiryDate <= ExpiryDate)
            throw new DomainException("Yeni son kullanma tarihi mevcut tarihten sonra olmalıdır.");
        ExpiryDate = newExpiryDate;
        Status = LicenseStatus.Active;
    }
}