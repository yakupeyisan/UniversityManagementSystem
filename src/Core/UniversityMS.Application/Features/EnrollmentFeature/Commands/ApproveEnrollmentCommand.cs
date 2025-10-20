using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public record ApproveEnrollmentCommand(
    Guid EnrollmentId,
    Guid AdvisorId,
    Guid ApprovedBy 
) : IRequest<Result>;