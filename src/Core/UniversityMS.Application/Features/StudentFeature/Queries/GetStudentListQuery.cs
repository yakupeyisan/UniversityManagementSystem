using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StudentFeature.DTOs;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.StudentFeature.Queries;

public record GetStudentListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    StudentStatus? Status = null,
    Guid? DepartmentId = null,
    string? SearchTerm = null
) : IRequest<Result<PaginatedList<StudentDto>>>;