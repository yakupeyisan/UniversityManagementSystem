using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMS.Application.Features.Departments.Commands;
using UniversityMS.Application.Features.Departments.Queries;

namespace UniversityMS.Api.Controllers.v1;

[Authorize]
public class DepartmentsController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartments(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? facultyId = null,
        [FromQuery] bool? isActive = null)
    {
        var query = new GetDepartmentListQuery(pageNumber, pageSize, facultyId, isActive);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDepartment(Guid id)
    {
        var query = new GetDepartmentByIdQuery(id);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetDepartment), new { id = result.Data }, result);
    }

    [HttpPost("{id:guid}/assign-head")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignHead(Guid id, [FromBody] AssignDepartmentHeadCommand command)
    {
        if (id != command.DepartmentId)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetDepartments(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? facultyId = null)
    {
        var result = await Mediator.Send(new GetDepartmentListQuery(pageNumber, pageSize, facultyId));
        return Ok(result);
    }

}