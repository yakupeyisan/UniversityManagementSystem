using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Staff.DTOs;

namespace UniversityMS.Application.Features.Staff.Queries;

public class GetEmployeeByIdQuery : IRequest<Result<EmployeeDto>>
{
    public Guid EmployeeId { get; set; }
}