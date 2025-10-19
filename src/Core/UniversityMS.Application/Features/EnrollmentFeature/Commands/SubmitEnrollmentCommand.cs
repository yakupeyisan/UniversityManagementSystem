using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public record SubmitEnrollmentCommand(Guid EnrollmentId) : IRequest<Result>;