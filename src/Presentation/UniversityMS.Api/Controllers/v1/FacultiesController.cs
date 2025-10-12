using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMS.Application.Features.Faculties.Commands;
using UniversityMS.Application.Features.Faculties.Queries;

namespace UniversityMS.Api.Controllers.v1;

[Authorize]
public class FacultiesController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFaculties(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null)
    {
        var query = new GetFacultyListQuery(pageNumber, pageSize, isActive);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFaculty(Guid id)
    {
        var query = new GetFacultyByIdQuery(id);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetFaculty), new { id = result.Data }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFaculty(Guid id, [FromBody] UpdateFacultyCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteFaculty(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
        var command = new DeleteFacultyCommand(id, userId);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return NotFound(result);

        return NoContent();
    }
    [HttpGet]
    public async Task<IActionResult> GetFaculties(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await Mediator.Send(new GetFacultyListQuery(pageNumber, pageSize));
        return Ok(result);
    }

}