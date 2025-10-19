using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public record RejectEnrollmentCommand(
    Guid EnrollmentId,
    string RejectionReason  
) : IRequest<Result>;
