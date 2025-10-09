using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniversityMS.Application.Features.Attendances.Commands;
using UniversityMS.Application.Features.Attendances.Queries;
using UniversityMS.Application.Features.Courses.Commands;
using UniversityMS.Application.Features.Courses.Queries;
using UniversityMS.Application.Features.Departments.Commands;
using UniversityMS.Application.Features.Departments.Queries;
using UniversityMS.Application.Features.Enrollments.Commands;
using UniversityMS.Application.Features.Enrollments.Queries;
using UniversityMS.Application.Features.Faculties.Commands;
using UniversityMS.Application.Features.Faculties.Queries;
using UniversityMS.Application.Features.Grades.Commands;
using UniversityMS.Application.Features.Grades.Queries;
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
}

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
}

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
}

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
        if (id != command.Id)
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
}

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