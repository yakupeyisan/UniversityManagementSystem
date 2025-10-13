using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Classrooms.Commands;

public record ToggleClassroomActiveCommand(Guid Id) : IRequest<Result>;