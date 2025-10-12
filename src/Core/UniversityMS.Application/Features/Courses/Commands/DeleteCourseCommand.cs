using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Courses.Commands;

public record DeleteCourseCommand(Guid Id) : IRequest<Result>;