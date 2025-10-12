using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Grades.DTOs;

namespace UniversityMS.Application.Features.Grades.Commands;

public record BulkCreateGradesCommand(
    List<GradeDto> Grades
) : IRequest<Result<int>>;