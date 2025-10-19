namespace UniversityMS.Application.Features.Inventory.DTOs;

public class StockMovementDto
{
    public Guid Id { get; set; }
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public Guid StockItemId { get; set; }
    public decimal Quantity { get; set; }
    public DateTime MovementDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ValuationMethod { get; set; } = string.Empty; // FIFO, LIFO
    public string Status { get; set; } = string.Empty;
    public string? MovementType { get; set; }      // Type.ToString()
    public string? ReferenceNumber { get; set; }   // Reason field
}