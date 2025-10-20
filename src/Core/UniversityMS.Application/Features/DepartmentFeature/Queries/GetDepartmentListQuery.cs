using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.DepartmentFeature.DTOs;

namespace UniversityMS.Application.Features.DepartmentFeature.Queries;

public record GetDepartmentListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Filter = null  
) : IRequest<Result<PaginatedList<DepartmentDto>>>;