using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Faculties.Commands;

public record DeleteFacultyCommand(Guid Id, string DeletedBy) : IRequest<Result>;