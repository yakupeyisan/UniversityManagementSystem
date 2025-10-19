using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public record AddCourseToEnrollmentCommand(
    Guid EnrollmentId,
    Guid CourseId
) : IRequest<Result>;