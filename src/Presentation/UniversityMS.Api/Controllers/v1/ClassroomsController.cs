using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UniversityMS.Api.Controllers.v1;

[Authorize]
public class ClassroomsController : BaseApiController
{
    /// <summary>
    /// Tüm derslikleri listele
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClassrooms(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? building = null,
        [FromQuery] bool? isActive = null)
    {
        var query = new GetClassroomListQuery(pageNumber, pageSize, building, isActive);
        var result = await Mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Derslik detayı
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClassroom(Guid id)
    {
        var query = new GetClassroomByIdQuery(id);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Yeni derslik oluştur
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateClassroom([FromBody] CreateClassroomCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetClassroom), new { id = result.Data }, result);
    }

    /// <summary>
    /// Derslik bilgilerini güncelle
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateClassroom(Guid id, [FromBody] UpdateClassroomCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Müsait derslikleri bul
    /// </summary>
    [HttpPost("available")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> FindAvailableClassrooms([FromBody] FindAvailableClassroomsQuery query)
    {
        var result = await Mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Derslik programını getir
    /// </summary>
    [HttpGet("{id:guid}/schedule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClassroomSchedule(
        Guid id,
        [FromQuery] string academicYear,
        [FromQuery] int semester)
    {
        var query = new GetClassroomScheduleQuery(id, academicYear, semester);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Dersliği aktif/pasif yap
    /// </summary>
    [HttpPatch("{id:guid}/toggle-active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var command = new ToggleClassroomActiveCommand(id);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}