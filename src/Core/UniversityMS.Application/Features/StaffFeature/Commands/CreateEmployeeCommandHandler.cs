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

            var staff = Staff.Create(
                request.FirstName,
                request.LastName,
                email,
                phoneNumber,
                request.Position,
                request.DepartmentId,
                request.EmploymentType,
                request.HireDate);

            await _staffRepository.AddAsync(staff, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Employee created. StaffId: {StaffId}, Email: {Email}",
                staff.Id, request.Email);

            return Result<Guid>.Success(staff.Id, "Çalışan başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return Result<Guid>.Failure("Çalışan oluşturulurken bir hata oluştu.", ex.Message);
        }
    }
}