using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Students.Commands;

public record DeleteStudentCommand(Guid Id) : IRequest<Result>;