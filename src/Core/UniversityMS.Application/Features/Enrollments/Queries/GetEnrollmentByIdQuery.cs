using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Enrollments.DTOs;

namespace UniversityMS.Application.Features.Enrollments.Queries;


public record GetEnrollmentByIdQuery(Guid Id) : IRequest<Result<EnrollmentDto>>;