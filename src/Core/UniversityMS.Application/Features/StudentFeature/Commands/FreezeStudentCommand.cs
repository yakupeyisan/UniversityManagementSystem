using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public record FreezeStudentCommand(Guid StudentId) : IRequest<Result>;