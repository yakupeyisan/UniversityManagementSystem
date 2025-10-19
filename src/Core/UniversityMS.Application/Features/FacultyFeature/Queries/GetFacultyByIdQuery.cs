using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FacultyFeature.DTOs;

namespace UniversityMS.Application.Features.FacultyFeature.Queries;

public record GetFacultyByIdQuery(Guid Id) : IRequest<Result<FacultyDto>>;