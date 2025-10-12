using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public record RejectEnrollmentCommand(
    Guid EnrollmentId,
    Guid AdvisorId,
    string Reason
) : IRequest<Result>;