using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMS.Application.Features.Attendances.Commands;
using UniversityMS.Application.Features.Attendances.Queries;
using UniversityMS.Application.Features.Authentication.Commands;

namespace UniversityMS.Api.Controllers.v1;

[Authorize]
public class AttendancesController : BaseApiController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> TakeAttendance([FromBody] TakeAttendanceCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("qr-check-in")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> QRCheckIn([FromBody] QRCheckInCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("student/{studentId:guid}/course/{courseId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentAttendance(Guid studentId, Guid courseId)
    {
        var query = new GetStudentAttendanceQuery(studentId, courseId);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("course/{courseId:guid}/report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttendanceReport(
        Guid courseId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = new GetAttendanceReportQuery(courseId, startDate, endDate);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}

