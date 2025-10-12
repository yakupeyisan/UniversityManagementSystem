using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Attendances.Commands;

public record QRCheckInCommand(
    Guid StudentId,
    Guid CourseId,
    Guid CourseRegistrationId,
    string QRCode,
    int WeekNumber
) : IRequest<Result>;