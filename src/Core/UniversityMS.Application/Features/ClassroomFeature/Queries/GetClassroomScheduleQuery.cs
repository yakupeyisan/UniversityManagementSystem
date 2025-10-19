using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ClassroomFeature.DTOs;

namespace UniversityMS.Application.Features.ClassroomFeature.Queries;

public record GetClassroomScheduleQuery(
    Guid ClassroomId
) : IRequest<Result<ClassroomScheduleDto>>;