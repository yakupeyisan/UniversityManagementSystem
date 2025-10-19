using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public record CreateGradeObjectionCommand(
    Guid GradeId,
    Guid StudentId,
    string Reason
) : IRequest<Result<Guid>>;
