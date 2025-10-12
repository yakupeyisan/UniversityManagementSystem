using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public record SubmitEnrollmentCommand(Guid EnrollmentId) : IRequest<Result>;