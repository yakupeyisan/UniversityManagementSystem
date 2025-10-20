using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FacultyFeature.DTOs;

namespace UniversityMS.Application.Features.FacultyFeature.Queries;
public record GetFacultyListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Filter = null  
) : IRequest<Result<PaginatedList<FacultyDto>>>;