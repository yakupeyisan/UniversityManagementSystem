using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;

namespace UniversityMS.Application.Features.ClassroomFeature.Queries;

public record GetClassroomListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Filter = null,
    string? OrderBy = null 
) : IRequest<Result<PaginatedList<ClassroomDto>>>;