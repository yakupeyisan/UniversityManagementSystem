using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.HRFeature.Commands;

public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, Result<Guid>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IRepository<Contract> _contractRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateContractCommandHandler> _logger;

    public CreateContractCommandHandler(
        IRepository<Employee> employeeRepository,
        IRepository<Contract> contractRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateContractCommandHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateContractCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
            if (employee == null)
                return Result<Guid>.Failure("Çalışan bulunamadı.");

            var contractType = Enum.Parse<ContractType>(request.ContractType);
            var salary = SalaryInfo.Create(request.BaseSalary, "TRY");
            var workingHours = WorkingHours.CreateStandard();

            var contract = Contract.Create(
                request.EmployeeId,
                request.ContractNumber,
                contractType,
                request.StartDate,
                salary,
                workingHours,
                request.EndDate,
                request.Terms);

            employee.AddContract(contract);

            await _contractRepository.AddAsync(contract, cancellationToken);
            await _employeeRepository.UpdateAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Contract created: {ContractNumber} for Employee {EmployeeId}",
                request.ContractNumber, request.EmployeeId);

            return Result<Guid>.Success(contract.Id, "Sözleşme başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contract");
            return Result<Guid>.Failure($"Sözleşme oluşturulurken hata: {ex.Message}");
        }
    }
}