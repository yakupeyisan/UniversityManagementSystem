using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMS.Application.Features.Courses.Commands;
using UniversityMS.Application.Features.Courses.Queries;
using UniversityMS.Application.Features.Grades.Commands;

namespace UniversityMS.Api.Controllers.v1;

[Authorize]
public class CoursesController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null)
    {
        var query = new GetCourseListQuery(pageNumber, pageSize, departmentId, searchTerm, isActive);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        var query = new GetCourseByIdQuery(id);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetCourse), new { id = result.Data }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{id:guid}/prerequisites")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddPrerequisite(Guid id, [FromBody] AddPrerequisiteCommand command)
    {
        if (id != command.CourseId)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:guid}/prerequisites")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPrerequisites(Guid id)
    {
        var query = new GetCoursePrerequisitesQuery(id);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
    /// <summary>
    /// Ders sil (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var command = new DeleteCourseCommand(id);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return NoContent();
    }

    /// <summary>
    /// Dersi aktif/pasif yap
    /// </summary>
    [HttpPatch("{id:guid}/active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleCourseActive(
        Guid id,
        [FromBody] ToggleCourseActiveCommand command)
    {
        if (id != command.CourseId)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Ön koşul kaldır
    /// </summary>
    [HttpDelete("{id:guid}/prerequisites/{prerequisiteId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemovePrerequisite(Guid id, Guid prerequisiteId)
    {
        var command = new RemovePrerequisiteCommand(id, prerequisiteId);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetCourses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] string? searchTerm = null)
    {
        var result = await Mediator.Send(new GetCourseListQuery(
            pageNumber, pageSize, departmentId, searchTerm));
        return Ok(result);
    }
}