using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public record RemoveCourseFromEnrollmentCommand(
    Guid EnrollmentId,
    Guid CourseId
) : IRequest<Result>;