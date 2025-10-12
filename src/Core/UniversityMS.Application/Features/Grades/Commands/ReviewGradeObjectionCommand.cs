using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Grades.Commands;

public record ReviewGradeObjectionCommand(
    Guid ObjectionId,
    Guid ReviewedBy,
    bool IsApproved,
    double? NewScore = null,
    string? ReviewNotes = null
) : IRequest<Result>;