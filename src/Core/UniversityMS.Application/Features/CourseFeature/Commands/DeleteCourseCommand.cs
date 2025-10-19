using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.CourseFeature.Commands;

public record DeleteCourseCommand(Guid Id) : IRequest<Result>;