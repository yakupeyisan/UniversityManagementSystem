using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Faculties.DTOs;

namespace UniversityMS.Application.Features.Faculties.Queries;

public record GetFacultyListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    bool? IsActive = null
) : IRequest<Result<PaginatedList<FacultyDto>>>;