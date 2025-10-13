using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMS.Application.Features.Schedules.Commands;
using UniversityMS.Application.Features.Schedules.Queries;

namespace UniversityMS.Api.Controllers.v1;

[Authorize]
public class SchedulesController : BaseApiController
{
    /// <summary>
    /// Yeni ders programı oluştur
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetSchedule), new { id = result.Data }, result);
    }

    /// <summary>
    /// Program detayını getir
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSchedule(Guid id)
    {
        var query = new GetScheduleByIdQuery(id);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Haftalık program görünümü
    /// </summary>
    [HttpGet("{id:guid}/weekly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWeeklySchedule(
        Guid id,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] Guid? instructorId = null,
        [FromQuery] Guid? studentId = null)
    {
        var query = new GetWeeklyScheduleQuery(id, departmentId, instructorId, studentId);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Programa ders ekle
    /// </summary>
    [HttpPost("{id:guid}/sessions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddCourseSession(Guid id, [FromBody] AddCourseSessionCommand command)
    {
        if (id != command.ScheduleId)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Programı yayınla
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PublishSchedule(Guid id)
    {
        var command = new PublishScheduleCommand(id);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Programı aktif hale getir
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ActivateSchedule(Guid id)
    {
        var command = new ActivateScheduleCommand(id);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Çakışma kontrolü yap
    /// </summary>
    [HttpPost("{id:guid}/check-conflicts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckConflicts(Guid id, [FromBody] CheckScheduleConflictsCommand command)
    {
        if (id != command.ScheduleId)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        return Ok(result);
    }

    /// <summary>
    /// Öğretim üyesi ders yükünü getir
    /// </summary>
    [HttpGet("instructor/{instructorId:guid}/workload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInstructorWorkload(
        Guid instructorId,
        [FromQuery] string academicYear,
        [FromQuery] int semester)
    {
        var query = new GetInstructorWorkloadQuery(instructorId, academicYear, semester);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}