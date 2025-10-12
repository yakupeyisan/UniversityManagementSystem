using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Students.Commands;

public record GraduateStudentCommand(Guid StudentId) : IRequest<Result>;