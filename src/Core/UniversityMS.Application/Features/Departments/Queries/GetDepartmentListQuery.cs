using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Departments.DTOs;

namespace UniversityMS.Application.Features.Departments.Queries;


public record GetDepartmentListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? FacultyId = null,
    bool? IsActive = null
) : IRequest<Result<PaginatedList<DepartmentDto>>>;