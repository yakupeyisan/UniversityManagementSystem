using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HR.DTOs;

namespace UniversityMS.Application.Features.HR.Queries;

public record GetEmployeeByIdQuery(Guid Id) : IRequest<Result<EmployeeDto>>;