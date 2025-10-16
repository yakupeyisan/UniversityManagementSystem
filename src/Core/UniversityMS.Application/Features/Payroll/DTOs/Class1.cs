namespace UniversityMS.Application.Features.Payroll.DTOs;

public record PayrollDeductionDto(
    string Type,
    decimal Amount,
    string? Description
);