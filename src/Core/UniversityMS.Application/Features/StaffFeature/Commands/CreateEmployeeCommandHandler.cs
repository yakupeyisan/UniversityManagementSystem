using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Application.Features.StaffFeature.Commands;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
{
    private readonly IRepository<Staff> _staffRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateEmployeeCommandHandler> _logger;

    public CreateEmployeeCommandHandler(
        IRepository<Staff> staffRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateEmployeeCommandHandler> logger)
    {
        _staffRepository = staffRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var email = Email.Create(request.Email);
            var phoneNumber = PhoneNumber.Create(request.PhoneNumber);

            // Staff.Create gerçek parametreleri:
            // firstName, lastName, nationalId, birthDate, gender, 
            // email, phoneNumber, employeeNumber, jobTitle, hireDate, departmentId, academicTitle

            var staff = Staff.Create(
                firstName: request.FirstName,
                lastName: request.LastName,
                nationalId: "00000000000",  // Placeholder - request'de yok
                birthDate: DateTime.Now.AddYears(-25),  // Placeholder - request'de yok
                gender: Domain.Enums.Gender.Male,  // Placeholder - request'de yok
                email: email,
                phoneNumber: phoneNumber,
                employeeNumber: Guid.NewGuid().ToString().Substring(0, 8),  // Auto generate
                jobTitle: request.Position,
                hireDate: request.HireDate,
                departmentId: request.DepartmentId != Guid.Empty ? request.DepartmentId : null,
                academicTitle: null
            );

            await _staffRepository.AddAsync(staff, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Employee created. StaffId: {StaffId}", staff.Id);
            return Result<Guid>.Success(staff.Id, "Çalışan başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return Result<Guid>.Failure("Çalışan oluşturulurken bir hata oluştu.");
        }
    }
}
