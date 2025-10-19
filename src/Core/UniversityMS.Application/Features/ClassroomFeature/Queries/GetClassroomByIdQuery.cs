using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;

namespace UniversityMS.Application.Features.ClassroomFeature.Queries;

public record GetClassroomByIdQuery(Guid Id) : IRequest<Result<ClassroomDto>>;
