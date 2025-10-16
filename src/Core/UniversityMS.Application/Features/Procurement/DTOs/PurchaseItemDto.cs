namespace UniversityMS.Application.Features.Procurement.DTOs;

public record PurchaseItemDto(
    string ItemName,
    int Quantity,
    decimal UnitPrice,
    string Unit,
    string? Description,
    string? Specifications
);