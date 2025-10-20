using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StudentFeature.DTOs;

namespace UniversityMS.Application.Features.StudentFeature.Queries;

public record GetStudentListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Filter = null 
) : IRequest<Result<PaginatedList<StudentDto>>>;