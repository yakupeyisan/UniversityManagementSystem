using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMS.Application.Features.Staff.Commands;
using UniversityMS.Application.Features.Staff.Queries;

namespace UniversityMS.Api.Controllers.v1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class EmployeesController : BaseApiController
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Yeni çalışan oluştur
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEmployee(
        [FromBody] CreateEmployeeCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetEmployee), new { id = result.Data?.Id }, result.Data);

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Çalışan detaylarını getir
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployee(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEmployeeByIdQuery { EmployeeId = id }, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Data);

        return NotFound(result.Errors);
    }

    /// <summary>
    /// Çalışanları arama
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchEmployees(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? departmentId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new SearchEmployeesQuery
            {
                SearchTerm = searchTerm,
                DepartmentId = departmentId,
                PageNumber = pageNumber,
                PageSize = pageSize
            },
            cancellationToken);

        return Ok(result.Data);
    }

    /// <summary>
    /// İzin talebinde bulun
    /// </summary>
    [HttpPost("{employeeId}/leaves")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApplyLeave(
        Guid employeeId,
        [FromBody] ApplyLeaveCommand command,
        CancellationToken cancellationToken)
    {
        command.EmployeeId = employeeId;
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetEmployee), new { id = employeeId }, result.Data);

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// İzin bakiyesini kontrol et
    /// </summary>
    [HttpGet("{employeeId}/leave-balance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaveBalance(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetLeaveBalanceQuery { EmployeeId = employeeId },
            cancellationToken);

        return Ok(result.Data);
    }

    /// <summary>
    /// Yoklama kaydını oluştur
    /// </summary>
    [HttpPost("{employeeId}/attendance")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordAttendance(
        Guid employeeId,
        [FromBody] RecordAttendanceCommand command,
        CancellationToken cancellationToken)
    {
        command.EmployeeId = employeeId;
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetEmployee), new { id = employeeId }, result.Data);

        return BadRequest(result.Errors);
    }
}