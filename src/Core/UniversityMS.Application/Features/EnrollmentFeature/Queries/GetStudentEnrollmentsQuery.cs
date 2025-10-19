using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.EnrollmentFeature.DTOs;

namespace UniversityMS.Application.Features.EnrollmentFeature.Queries;

public record GetStudentEnrollmentsQuery(
    Guid StudentId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedList<EnrollmentDto>>>;