using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;

namespace UniversityMS.Application.Features.Classrooms.Queries;

public record GetClassroomListQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Building = null,
    bool? IsActive = null
) : IRequest<Result<PaginatedList<ClassroomDto>>>;