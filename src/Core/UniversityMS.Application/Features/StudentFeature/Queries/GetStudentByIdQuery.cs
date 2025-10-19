using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StudentFeature.DTOs;

namespace UniversityMS.Application.Features.StudentFeature.Queries;

public record GetStudentByIdQuery(Guid Id) : IRequest<Result<StudentDto>>;