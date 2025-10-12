using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Grades.Commands;

public record UpdateGradeCommand(
    Guid GradeId,
    double NumericScore,
    string? Notes = null
) : IRequest<Result>;