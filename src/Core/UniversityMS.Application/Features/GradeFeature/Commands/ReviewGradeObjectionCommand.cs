using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public record ReviewGradeObjectionCommand(
    Guid ObjectionId,
    bool IsApproved,
    string ReviewNotes
) : IRequest<Result>;