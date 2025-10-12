using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Students.DTOs;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Students.Queries;

public record GetStudentListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    StudentStatus? Status = null,
    Guid? DepartmentId = null,
    string? SearchTerm = null
) : IRequest<Result<PaginatedList<StudentDto>>>;