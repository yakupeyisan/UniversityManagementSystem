namespace UniversityMS.Application.Common.Interfaces;

/// <summary>
/// İşsizlik Sigortası DTO
/// </summary>
public class UnemploymentInsuranceDto
{
    /// <summary>Çalışan payı (%1)</summary>
    public decimal EmployeeRate { get; set; }

    /// <summary>Çalışan payı tutarı</summary>
    public decimal EmployeeAmount { get; set; }

    /// <summary>İşveren payı (%2)</summary>
    public decimal EmployerRate { get; set; }

    /// <summary>İşveren payı tutarı</summary>
    public decimal EmployerAmount { get; set; }

    /// <summary>Toplam prim</summary>
    public decimal TotalAmount { get; set; }
}