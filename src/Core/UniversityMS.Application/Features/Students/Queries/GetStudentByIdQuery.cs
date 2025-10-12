using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Students.DTOs;

namespace UniversityMS.Application.Features.Students.Queries;

public record GetStudentByIdQuery(Guid Id) : IRequest<Result<StudentDto>>;