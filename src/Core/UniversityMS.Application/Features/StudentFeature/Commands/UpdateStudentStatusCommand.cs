using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public record UpdateStudentStatusCommand(
    Guid StudentId,
    StudentStatus Status
) : IRequest<Result>;