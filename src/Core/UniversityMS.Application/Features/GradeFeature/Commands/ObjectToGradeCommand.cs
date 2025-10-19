using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public record ObjectToGradeCommand(
    Guid GradeId,
    Guid StudentId,
    string Reason,
    string Description
) : IRequest<Result<Guid>>;