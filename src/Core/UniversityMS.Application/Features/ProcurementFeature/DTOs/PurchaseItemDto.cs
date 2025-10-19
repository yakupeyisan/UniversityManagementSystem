namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

public record PurchaseItemDto(
    string ItemName,
    int Quantity,
    decimal UnitPrice,
    string Unit,
    string? Description,
    string? Specifications
);