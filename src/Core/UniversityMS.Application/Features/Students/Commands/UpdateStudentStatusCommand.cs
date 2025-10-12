using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Students.Commands;

public record UpdateStudentStatusCommand(
    Guid StudentId,
    StudentStatus Status
) : IRequest<Result>;