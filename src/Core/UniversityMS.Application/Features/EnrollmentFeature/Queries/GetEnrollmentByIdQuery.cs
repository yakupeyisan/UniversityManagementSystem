using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.EnrollmentFeature.DTOs;

namespace UniversityMS.Application.Features.EnrollmentFeature.Queries;


public record GetEnrollmentByIdQuery(Guid Id) : IRequest<Result<EnrollmentDto>>;