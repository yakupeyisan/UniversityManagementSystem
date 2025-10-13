using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMS.Application.Features.Grades.Commands;
using UniversityMS.Application.Features.Grades.Queries;

namespace UniversityMS.Api.Controllers.v1;

[Authorize]
public class GradesController : BaseApiController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetStudentGrades), new { studentId = command.StudentId }, result);
    }
    [HttpPost("SubmitGrade")]
    public async Task<IActionResult> SubmitGrade([FromBody] SubmitGradeCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("bulk")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> BulkCreateGrades([FromBody] BulkCreateGradesCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("student/{studentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentGrades(Guid studentId, [FromQuery] Guid? courseId = null)
    {
        var query = new GetStudentGradesQuery(studentId, courseId);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("course/{courseId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseGrades(Guid courseId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var query = new GetCourseGradesQuery(courseId, pageNumber, pageSize);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("course/{courseId:guid}/statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGradeStatistics(Guid courseId)
    {
        var query = new GetGradeStatisticsQuery(courseId);
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
    /// <summary>
    /// Not güncelle
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGrade(Guid id, [FromBody] UpdateGradeCommand command)
    {
        if (id != command.GradeId)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Not sil
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGrade(Guid id)
    {
        var command = new DeleteGradeCommand(id);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return NoContent();
    }

    /// <summary>
    /// Not itirazı oluştur
    /// </summary>
    [HttpPost("objections")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateGradeObjection([FromBody] CreateGradeObjectionCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetGradeObjection), new { id = result.Data }, result);
    }

    /// <summary>
    /// Not itirazını incele (onayla/reddet)
    /// </summary>
    [HttpPost("objections/{id:guid}/review")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReviewGradeObjection(
        Guid id,
        [FromBody] ReviewGradeObjectionCommand command)
    {
        if (id != command.ObjectionId)
            return BadRequest("ID uyuşmazlığı");

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Not itirazını getir
    /// </summary>
    [HttpGet("objections/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGradeObjection(Guid id)
    {
        // GetGradeObjectionQuery implementation
        // var query = new GetGradeObjectionQuery(id);
        // var result = await Mediator.Send(query);

        return Ok(new { message = "Not itirazı endpoint - implement edilecek" });
    }

    /// <summary>
    /// Bekleyen not itirazlarını listele
    /// </summary>
    [HttpGet("objections/pending")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingGradeObjections(
        [FromQuery] Guid? courseId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // GetPendingGradeObjectionsQuery implementation
        return Ok(new { message = "Bekleyen itirazlar endpoint - implement edilecek" });
    }


    [HttpGet("transcript/{studentId:guid}")]
    public async Task<IActionResult> GetTranscript(Guid studentId)
    {
        var result = await Mediator.Send(new GetTranscriptQuery(studentId));
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost("objection")]
    public async Task<IActionResult> ObjectToGrade([FromBody] ObjectToGradeCommand command)
    {
        var result = await Mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPut("objection/{objectionId:guid}/review")]
    public async Task<IActionResult> ReviewObjection(Guid objectionId, [FromBody] ReviewGradeObjectionCommand command)
    {
        if (objectionId != command.ObjectionId)
            return BadRequest("ID mismatch");
        var result = await Mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}