using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Departments.Commands;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<Guid>>
{
    private readonly IRepository<Department> _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDepartmentCommandHandler> _logger;

    public CreateDepartmentCommandHandler(
        IRepository<Department> departmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateDepartmentCommandHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var department = Department.Create(
                request.Name,
                request.Code,
                request.FacultyId,
                request.Description
            );

            await _departmentRepository.AddAsync(department, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(department.Id, "Bölüm oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating department");
            return Result.Failure<Guid>("Bölüm oluşturulamadı.");
        }
    }
}