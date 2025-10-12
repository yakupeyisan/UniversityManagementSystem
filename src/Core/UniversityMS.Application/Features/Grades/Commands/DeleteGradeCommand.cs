using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Grades.Commands;

public record DeleteGradeCommand(Guid Id) : IRequest<Result>;