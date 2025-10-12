using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Courses.Commands;



public record CreateCourseCommand(
    string Name,
    string Code,
    string? Description,
    Guid DepartmentId,
    CourseType CourseType,
    int TheoreticalHours,
    int PracticalHours,
    int ECTS,
    int NationalCredit,
    EducationLevel EducationLevel,
    int? Semester
) : IRequest<Result<Guid>>;