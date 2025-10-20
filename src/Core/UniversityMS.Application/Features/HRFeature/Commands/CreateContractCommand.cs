using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public record CreateContractCommand(
    Guid EmployeeId,
    string ContractNumber,
    string ContractType,
    DateTime StartDate,
    DateTime? EndDate,
    decimal BaseSalary,
    string? Terms = null
) : IRequest<Result<Guid>>;