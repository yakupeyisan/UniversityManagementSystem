using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMS.Application.Features.Inventory.Commands;
using UniversityMS.Application.Features.Inventory.Queries;

namespace UniversityMS.Api.Controllers.v1;

[Authorize(Roles = "Admin,Warehouse")]
[Route("api/v1/[controller]")]
[ApiController]
public class InventoryController : BaseApiController
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Yeni stok kalemi oluştur
    /// </summary>
    [HttpPost("items")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStockItem(
        [FromBody] CreateStockItemCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetStockItem), new { id = result.Data?.Id }, result.Data);

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Stok hareketini kaydet (Depo transfer, FIFO/LIFO)
    /// </summary>
    [HttpPost("movements")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MoveStock(
        [FromBody] MoveStockCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetMovement), new { id = result.Data?.Id }, result.Data);

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Stok seviyesini kontrol et
    /// </summary>
    [HttpGet("levels")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockLevels(
        [FromQuery] Guid warehouseId,
        [FromQuery] string? category,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetStockLevelQuery
            {
                WarehouseId = warehouseId,
                Category = category
            },
            cancellationToken);

        return Ok(result.Data);
    }

    /// <summary>
    /// Depo envanter raporu
    /// </summary>
    [HttpGet("reports/{warehouseId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInventoryReport(
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetInventoryReportQuery { WarehouseId = warehouseId },
            cancellationToken);

        return Ok(result.Data);
    }

    private async Task<IActionResult> GetStockItem(Guid id)
    {
        // Implementation
        return Ok();
    }

    private async Task<IActionResult> GetMovement(Guid id)
    {
        // Implementation
        return Ok();
    }
}