using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Departments.Commands;


public record CreateDepartmentCommand(
    string Name,
    string Code,
    Guid FacultyId,
    string? Description = null
) : IRequest<Result<Guid>>;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FacultyId).NotEmpty();
    }
}

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


public record AssignDepartmentHeadCommand(
    Guid DepartmentId,
    Guid FacultyId
) : IRequest<Result>;

public class AssignDepartmentHeadCommandValidator : AbstractValidator<AssignDepartmentHeadCommand>
{
    public AssignDepartmentHeadCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Bölüm ID gereklidir.");

        RuleFor(x => x.FacultyId)
            .NotEmpty().WithMessage("Öğretim üyesi ID gereklidir.");
    }
}

public class AssignDepartmentHeadCommandHandler : IRequestHandler<AssignDepartmentHeadCommand, Result>
{
    private readonly IRepository<Department> _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignDepartmentHeadCommandHandler> _logger;

    public AssignDepartmentHeadCommandHandler(
        IRepository<Department> departmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignDepartmentHeadCommandHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(AssignDepartmentHeadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
            if (department == null)
                return Result.Failure("Bölüm bulunamadı.");

            department.AssignHead(request.FacultyId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Department head assigned: {DepartmentId} - {FacultyId}",
                request.DepartmentId, request.FacultyId);

            return Result.Success("Bölüm başkanı başarıyla atandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning department head");
            return Result.Failure("Bölüm başkanı atanırken bir hata oluştu.", ex.Message);
        }
    }
}