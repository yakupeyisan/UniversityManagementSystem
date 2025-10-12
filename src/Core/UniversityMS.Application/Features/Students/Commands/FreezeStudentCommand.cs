using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Students.Commands;

public record FreezeStudentCommand(Guid StudentId) : IRequest<Result>;