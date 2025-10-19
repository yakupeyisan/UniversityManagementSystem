using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMS.Application.Features.PayrollFeature.Commands;
using UniversityMS.Application.Features.PayrollFeature.Queries;

namespace UniversityMS.Api.Controllers.v1;

[Authorize(Roles = "Admin,Finance")]
[Route("api/v1/[controller]")]
[ApiController]
public class PayrollsController : BaseApiController
{
    private readonly IMediator _mediator;

    public PayrollsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Bordro oluştur
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePayroll(
        [FromBody] CreatePayrollCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetPayroll), new { id = result.Data?.Id }, result.Data);

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Bordro hesapla
    /// </summary>
    [HttpPost("{payrollId}/calculate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CalculatePayroll(
        Guid payrollId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CalculatePayrollCommand { PayrollId = payrollId },
            cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Data);

        return NotFound(result.Errors);
    }

    /// <summary>
    /// Maaş bordrosu oluştur
    /// </summary>
    [HttpPost("{payrollId}/generate-payslip")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GeneratePayslip(
        Guid payrollId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GeneratePayslipCommand { PayrollId = payrollId },
            cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetPayroll), new { id = payrollId }, result.Data);

        return NotFound(result.Errors);
    }

    /// <summary>
    /// Bordro detaylarını getir
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayroll(Guid id)
    {
        // Implementation
        return Ok();
    }

    /// <summary>
    /// Çalışan bordro geçmişini getir
    /// </summary>
    [HttpGet("employee/{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeePayrolls(
        Guid employeeId,
        [FromQuery] int? month,
        [FromQuery] int? year,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetPayrollByEmployeeQuery
            {
                EmployeeId = employeeId,
                Month = month,
                Year = year
            },
            cancellationToken);

        return Ok(result.Data);
    }

    /// <summary>
    /// Aylık bordro raporu
    /// </summary>
    [HttpGet("report/{year}/{month}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMonthlyReport(
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetMonthlyPayrollReportQuery { Month = month, Year = year },
            cancellationToken);

        return Ok(result.Data);
    }
}