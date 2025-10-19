using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.DepartmentFeature.Commands;

public class AssignDepartmentHeadCommandHandler : IRequestHandler<AssignDepartmentHeadCommand, Result>
{
    private readonly IRepository<Department> _departmentRepository;
    private readonly IRepository<Faculty> _facultyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignDepartmentHeadCommandHandler> _logger;

    public AssignDepartmentHeadCommandHandler(
        IRepository<Department> departmentRepository,
        IRepository<Faculty> facultyRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignDepartmentHeadCommandHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _facultyRepository = facultyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(AssignDepartmentHeadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
            if (department == null)
            {
                _logger.LogWarning("Department not found: {DepartmentId}", request.DepartmentId);
                return Result.Failure("Bölüm bulunamadı.");
            }

            var faculty = await _facultyRepository.GetByIdAsync(request.FacultyId, cancellationToken);
            if (faculty == null)
            {
                _logger.LogWarning("Faculty not found: {FacultyId}", request.FacultyId);
                return Result.Failure("Fakülte bulunamadı.");
            }

            // Bölüm müdürü atamasını yap
            department.AssignHead(request.FacultyId);

            await _departmentRepository.UpdateAsync(department, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Department head assigned. DepartmentId: {DepartmentId}, HeadId: {HeadId}",
                request.DepartmentId, request.FacultyId);

            return Result.Success("Bölüm müdürü başarıyla atandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning department head");
            return Result.Failure("Bölüm müdürü atanırken bir hata oluştu.", ex.Message);
        }
    }
}