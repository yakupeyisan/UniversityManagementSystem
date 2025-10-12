using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public record ApproveEnrollmentCommand(
    Guid EnrollmentId,
    Guid AdvisorId
) : IRequest<Result>;