using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StaffFeature.DTOs;

namespace UniversityMS.Application.Features.StaffFeature.Queries;

public record GetEmployeeByIdQuery(Guid Id) : IRequest<Result<EmployeeDto>>;