using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public record ObjectToGradeCommand(
    Guid GradeId,
    Guid StudentId,
    Guid CourseId,
    string Reason
) : IRequest<Result<Guid>>;