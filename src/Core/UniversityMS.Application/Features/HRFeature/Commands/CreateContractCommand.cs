using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// Sözleşme oluşturma command record'ı
/// </summary>
public record CreateContractCommand(
    Guid EmployeeId,
    string ContractNumber,
    string ContractType,
    DateTime StartDate,
    DateTime? EndDate,
    decimal BaseSalary,
    string Terms
) : IRequest<Result<Guid>>;