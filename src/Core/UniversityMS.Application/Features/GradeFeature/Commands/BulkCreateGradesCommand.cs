using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.GradeFeature.DTOs;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public record BulkCreateGradesCommand(
    List<GradeDto> Grades
) : IRequest<Result<int>>;