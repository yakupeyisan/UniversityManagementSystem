using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMS.Application.Features.Students.Commands;
using UniversityMS.Application.Features.Students.Queries;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Api.Controllers.v1;


[Authorize]
public class StudentsController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudents(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] StudentStatus? status = null,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetStudentListQuery(pageNumber, pageSize, status, departmentId, searchTerm);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudent(Guid id)
    {
        var query = new GetStudentByIdQuery(id);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetStudent), new { id = result.Data }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Öğrenci sil (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteStudent(Guid id)
    {
        var command = new DeleteStudentCommand(id);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return NoContent();
    }

    /// <summary>
    /// Öğrenci durumunu güncelle
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStudentStatus(
        Guid id,
        [FromBody] UpdateStudentStatusCommand command)
    {
        if (id != command.StudentId)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Öğrenci kayıt dondurma
    /// </summary>
    [HttpPost("{id:guid}/freeze")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> FreezeStudent(Guid id)
    {
        var command = new FreezeStudentCommand(id);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Öğrenciyi mezun et
    /// </summary>
    [HttpPost("{id:guid}/graduate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GraduateStudent(Guid id)
    {
        var command = new GraduateStudentCommand(id);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}