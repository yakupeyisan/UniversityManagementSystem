using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;

namespace UniversityMS.Application.Features.ScheduleFeature.Queries;

public record GetInstructorWorkloadQuery(
    Guid InstructorId,
    string AcademicYear,
    int Semester
) : IRequest<Result<InstructorWorkloadDto>>;