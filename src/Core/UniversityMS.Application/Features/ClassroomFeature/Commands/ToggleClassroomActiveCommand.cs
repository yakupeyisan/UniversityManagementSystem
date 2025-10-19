using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.ClassroomFeature.Commands;

public record ToggleClassroomActiveCommand(Guid Id) : IRequest<Result>;