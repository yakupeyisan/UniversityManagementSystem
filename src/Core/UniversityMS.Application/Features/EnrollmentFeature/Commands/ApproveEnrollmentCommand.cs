using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public record ApproveEnrollmentCommand(
    Guid EnrollmentId,
    Guid ApprovedBy  // Bu parameter var mı kontrol et
) : IRequest<Result>;
