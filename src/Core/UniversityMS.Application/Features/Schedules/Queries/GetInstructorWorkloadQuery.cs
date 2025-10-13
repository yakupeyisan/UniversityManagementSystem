using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;

namespace UniversityMS.Application.Features.Schedules.Queries;

public record GetInstructorWorkloadQuery(
    Guid InstructorId,
    string AcademicYear,
    int Semester
) : IRequest<Result<InstructorWorkloadDto>>;