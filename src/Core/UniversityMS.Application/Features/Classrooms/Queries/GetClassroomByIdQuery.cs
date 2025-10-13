using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;

namespace UniversityMS.Application.Features.Classrooms.Queries;

public record GetClassroomByIdQuery(Guid Id) : IRequest<Result<ClassroomDto>>;
