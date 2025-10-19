using Microsoft.AspNetCore.Mvc;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.Commands;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Application.Features.PayrollFeature.Queries;

namespace UniversityMS.Api.Controllers.v1;

/// <summary>
/// Bordro (Payroll) Yönetimi API
/// Aylık bordro hesaplama, onaylama, ödeme ve raporlama işlemleri
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Tags("Payroll")]
public class PayrollController : BaseApiController
{
    /// <summary>
    /// Yeni bordro oluştur
    /// </summary>
    /// <param name="command">Bordro oluşturma bilgileri</param>
    /// <returns>Oluşturulan bordro</returns>
    /// <response code="201">Bordro başarıyla oluşturuldu</response>
    /// <response code="400">Hatalı istek</response>
    /// <response code="409">Bu ay için bordro zaten mevcut</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePayroll(
        [FromBody] CreatePayrollCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetPayrollById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Bordroyu ID'ye göre getir
    /// </summary>
    /// <param name="id">Bordro ID</param>
    /// <returns>Bordro detayları</returns>
    /// <response code="200">Başarılı</response>
    /// <response code="404">Bordro bulunamadı</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayrollById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPayrollByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Çalışana ait tüm bordroları getir
    /// </summary>
    /// <param name="employeeId">Çalışan ID</param>
    /// <param name="year">Yıl (opsiyonel)</param>
    /// <param name="month">Ay (opsiyonel)</param>
    /// <param name="status">Durum (opsiyonel): Draft, Calculated, Approved, Paid</param>
    /// <returns>Bordro listesi</returns>
    /// <response code="200">Başarılı</response>
    /// <response code="404">Çalışan bulunamadı</response>
    [HttpGet("employee/{employeeId:guid}")]
    [ProducesResponseType(typeof(Result<List<PayrollDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<List<PayrollDto>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayrollByEmployee(
        [FromRoute] Guid employeeId,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var query = new GetPayrollByEmployeeQuery(employeeId, month, year);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Belirli ay için tüm bordroları getir
    /// </summary>
    /// <param name="year">Yıl</param>
    /// <param name="month">Ay (1-12)</param>
    /// <param name="status">Durum filtresi (opsiyonel)</param>
    /// <returns>Ay'ın tüm bordroları</returns>
    /// <response code="200">Başarılı</response>
    /// <response code="400">Hatalı ay/yıl</response>
    [HttpGet("month/{year:int}/{month:int}")]
    [ProducesResponseType(typeof(Result<List<PayrollDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<List<PayrollDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPayrollByMonth(
        [FromRoute] int year,
        [FromRoute] int month,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        if (month < 1 || month > 12)
            return BadRequest(Result<List<PayrollDto>>.Failure("Ay 1-12 arasında olmalıdır"));

        var query = new GetPayrollByMonthQuery(year, month, status);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Tüm bordroları sayfalama ile getir
    /// </summary>
    /// <param name="pageNumber">Sayfa numarası (varsayılan: 1)</param>
    /// <param name="pageSize">Sayfa boyutu (varsayılan: 50)</param>
    /// <param name="status">Durum filtresi (opsiyonel)</param>
    /// <param name="year">Yıl filtresi (opsiyonel)</param>
    /// <param name="month">Ay filtresi (opsiyonel)</param>
    /// <returns>Sayfalanmış bordro listesi</returns>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PaginatedList<PayrollDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPayrolls(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? status = null,
        [FromQuery] int? year = null,
        [FromQuery] int? month = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllPayrollsQuery(pageNumber, pageSize, status, year, month);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Bordroyu hesapla (Draft → Calculated)
    /// </summary>
    /// <param name="id">Bordro ID</param>
    /// <returns>Hesaplanan bordro</returns>
    /// <response code="200">Başarılı</response>
    /// <response code="400">Bordro hesaplanamadı</response>
    [HttpPost("{id:guid}/calculate")]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CalculatePayroll(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        // Not: CalculatePayrollCommand yazılması gerekli
        // Şimdilik GetPayrollById sonra Calculate yapılacak
        var query = new GetPayrollByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Bordroyu onayla (Calculated → Approved)
    /// </summary>
    /// <param name="id">Bordro ID</param>
    /// <returns>Onaylanan bordro</returns>
    /// <response code="200">Başarılı</response>
    /// <response code="400">Bordro onaylanamadı</response>
    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApprovePayroll(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ApprovePayrollCommand(id);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Bordro ödemesini işle (Approved → Paid)
    /// </summary>
    /// <param name="id">Bordro ID</param>
    /// <param name="command">Ödeme bilgileri</param>
    /// <returns>Ödenen bordro</returns>
    /// <response code="200">Başarılı</response>
    /// <response code="400">Ödeme işlenemedi</response>
    [HttpPost("{id:guid}/process-payment")]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPayment(
        [FromRoute] Guid id,
        [FromBody] ProcessPaymentCommand command,
        CancellationToken cancellationToken)
    {
        if (command.PayrollId != id)
            return BadRequest("Bordro ID uyuşmuyor");

        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Bordro raporunu (Payslip) getir
    /// PDF/Excel olarak indirilebilir
    /// </summary>
    /// <param name="id">Bordro ID</param>
    /// <returns>Bordro raporu</returns>
    /// <response code="200">Başarılı</response>
    /// <response code="404">Bordro bulunamadı</response>
    [HttpGet("{id:guid}/payslip")]
    [ProducesResponseType(typeof(Result<PayslipDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<PayslipDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GeneratePayslip(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPayslipQuery(id);
        var result = await Mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Ay için bordro özeti getir
    /// Toplam maaş, vergi, kesinti vb. istatistikler
    /// </summary>
    /// <param name="year">Yıl</param>
    /// <param name="month">Ay (1-12)</param>
    /// <returns>Bordro özeti</returns>
    /// <response code="200">Başarılı</response>
    [HttpGet("summary/{year:int}/{month:int}")]
    [ProducesResponseType(typeof(Result<PayrollSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayrollSummary(
        [FromRoute] int year,
        [FromRoute] int month,
        CancellationToken cancellationToken)
    {
        var query = new GetPayrollSummaryQuery(year, month);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Çalışan bordro istatistikleri
    /// Yıllık toplam, ortalama, YTD (Year-To-Date) veriler
    /// </summary>
    /// <param name="employeeId">Çalışan ID</param>
    /// <param name="year">Yıl</param>
    /// <returns>Çalışan istatistikleri</returns>
    /// <response code="200">Başarılı</response>
    [HttpGet("employee/{employeeId:guid}/statistics/{year:int}")]
    [ProducesResponseType(typeof(Result<EmployeePayrollStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeePayrollStatistics(
        [FromRoute] Guid employeeId,
        [FromRoute] int year,
        CancellationToken cancellationToken)
    {
        var query = new GetEmployeePayrollStatisticsQuery(employeeId, year);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Ödenmemiş/İşlenmeyen bordroları getir
    /// </summary>
    /// <param name="month">Ay filtresi (opsiyonel)</param>
    /// <param name="year">Yıl filtresi (opsiyonel)</param>
    /// <returns>Beklemede olan bordro listesi</returns>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(Result<List<PayrollDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingPayrolls(
        [FromQuery] int? month = null,
        [FromQuery] int? year = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPendingPayrollsQuery(month, year);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Bordro bilgilerini güncelle
    /// (Sadece Draft durumundaki bordro güncellenebilir)
    /// </summary>
    /// <param name="id">Bordro ID</param>
    /// <param name="command">Güncelleme bilgileri</param>
    /// <returns>Güncellenen bordro</returns>
    /// <response code="200">Başarılı</response>
    /// <response code="400">Güncellenemedi</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePayroll(
        [FromRoute] Guid id,
        [FromBody] UpdatePayrollDto dto,
        CancellationToken cancellationToken)
    {
        dto.PayrollId = id;

        // Note: UpdatePayrollCommand yazılması gerekli
        // Şimdilik placeholder
        var query = new GetPayrollByIdQuery(id);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Bordroyu sil
    /// (Sadece Draft durumundaki bordro silinebilir)
    /// </summary>
    /// <param name="id">Bordro ID</param>
    /// <returns>Başarılı sonuç</returns>
    /// <response code="204">Başarıyla silindi</response>
    /// <response code="400">Silinemedi</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result<PayrollDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePayroll(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        // Note: DeletePayrollCommand yazılması gerekli
        return NoContent();
    }
}