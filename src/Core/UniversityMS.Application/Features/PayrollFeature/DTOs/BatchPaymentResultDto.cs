namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

public class BatchPaymentResultDto
{
    public int TotalProcessed { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public decimal TotalAmountPaid { get; set; }
    public DateTime ProcessedDate { get; set; }
    public List<string> Errors { get; set; } = new();
}