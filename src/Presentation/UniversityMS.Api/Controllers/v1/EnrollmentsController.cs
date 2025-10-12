using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMS.Application.Features.Enrollments.Commands;
using UniversityMS.Application.Features.Enrollments.Queries;

namespace UniversityMS.Api.Controllers.v1;

[Authorize]
public class EnrollmentsController : BaseApiController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetEnrollment), new { id = result.Data }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrollment(Guid id)
    {
        var query = new GetEnrollmentByIdQuery(id);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost("{enrollmentId:guid}/courses")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddCourseToEnrollment(
        Guid enrollmentId,
        [FromBody] AddCourseToEnrollmentCommand command)
    {
        if (enrollmentId != command.EnrollmentId)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{enrollmentId:guid}/courses/{courseId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveCourseFromEnrollment(Guid enrollmentId, Guid courseId)
    {
        var command = new RemoveCourseFromEnrollmentCommand(enrollmentId, courseId);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{enrollmentId:guid}/submit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitEnrollment(Guid enrollmentId)
    {
        var command = new SubmitEnrollmentCommand(enrollmentId);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{enrollmentId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ApproveEnrollment(Guid enrollmentId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var command = new ApproveEnrollmentCommand(enrollmentId, userId);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{enrollmentId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectEnrollment(Guid enrollmentId, [FromBody] RejectEnrollmentCommand command)
    {
        if (enrollmentId != command.EnrollmentId)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("my-enrollments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyEnrollments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var studentId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
        var query = new GetStudentEnrollmentsQuery(studentId, pageNumber, pageSize);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}