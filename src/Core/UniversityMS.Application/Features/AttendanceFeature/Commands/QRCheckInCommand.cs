using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.AttendanceFeature.Commands;

public record QRCheckInCommand(
    Guid StudentId,
    Guid CourseId,
    Guid CourseRegistrationId,
    string QRCode,
    int WeekNumber
) : IRequest<Result>;