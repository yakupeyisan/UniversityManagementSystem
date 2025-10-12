using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Faculties.DTOs;

namespace UniversityMS.Application.Features.Faculties.Queries;

public record GetFacultyByIdQuery(Guid Id) : IRequest<Result<FacultyDto>>;