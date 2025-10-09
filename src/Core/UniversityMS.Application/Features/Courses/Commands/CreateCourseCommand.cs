using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Courses.Commands;



public record CreateCourseCommand(
    string Name,
    string Code,
    string? Description,
    Guid DepartmentId,
    CourseType CourseType,
    int TheoreticalHours,
    int PracticalHours,
    int ECTS,
    int NationalCredit,
    EducationLevel EducationLevel,
    int? Semester
) : IRequest<Result<Guid>>;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ders adı boş olamaz.")
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Ders kodu boş olamaz.")
            .MaximumLength(20);

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Bölüm seçilmelidir.");

        RuleFor(x => x.TheoreticalHours)
            .GreaterThanOrEqualTo(0).WithMessage("Teorik saat negatif olamaz.");

        RuleFor(x => x.PracticalHours)
            .GreaterThanOrEqualTo(0).WithMessage("Uygulama saati negatif olamaz.");

        RuleFor(x => x.ECTS)
            .GreaterThan(0).WithMessage("ECTS en az 1 olmalıdır.");

        RuleFor(x => x.Semester)
            .InclusiveBetween(1, 8).When(x => x.Semester.HasValue)
            .WithMessage("Dönem 1-8 arasında olmalıdır.");
    }
}

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Result<Guid>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<Department> _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCourseCommandHandler> _logger;

    public CreateCourseCommandHandler(
        IRepository<Course> courseRepository,
        IRepository<Department> departmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verify department exists
            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
            if (department == null)
                return Result.Failure<Guid>("Bölüm bulunamadı.");

            // Check if code exists
            var existingCourse = await _courseRepository.FirstOrDefaultAsync(
                c => c.Code == request.Code.Trim().ToUpperInvariant(),
                cancellationToken);

            if (existingCourse != null)
                return Result.Failure<Guid>($"'{request.Code}' kodu zaten kullanımda.");

            var course = Course.Create(
                request.Name,
                request.Code,
                request.DepartmentId,
                request.CourseType,
                request.TheoreticalHours,
                request.PracticalHours,
                request.ECTS,
                request.NationalCredit,
                request.EducationLevel,
                request.Semester,
                request.Description
            );

            await _courseRepository.AddAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course created: {CourseId} - {CourseName}", course.Id, course.Name);

            return Result.Success(course.Id, "Ders başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course");
            return Result.Failure<Guid>("Ders oluşturulurken bir hata oluştu.", ex.Message);
        }
    }
}


public record UpdateCourseCommand(
    Guid Id,
    string Name,
    int TheoreticalHours,
    int PracticalHours,
    int ECTS,
    int NationalCredit,
    int? Semester,
    string? Description
) : IRequest<Result<Guid>>;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ders adı boş olamaz.")
            .MaximumLength(200);

        RuleFor(x => x.TheoreticalHours)
            .GreaterThanOrEqualTo(0).WithMessage("Teorik saat negatif olamaz.");

        RuleFor(x => x.PracticalHours)
            .GreaterThanOrEqualTo(0).WithMessage("Uygulama saati negatif olamaz.");

        RuleFor(x => x.ECTS)
            .GreaterThan(0).WithMessage("ECTS en az 1 olmalıdır.");

        RuleFor(x => x.Semester)
            .InclusiveBetween(1, 8).When(x => x.Semester.HasValue)
            .WithMessage("Dönem 1-8 arasında olmalıdır.");
    }
}

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Result<Guid>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCourseCommandHandler> _logger;

    public UpdateCourseCommandHandler(
        IRepository<Course> courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
            if (course == null)
                return Result.Failure<Guid>("Ders bulunamadı.");

            course.Update(
                request.Name,
                request.TheoreticalHours,
                request.PracticalHours,
                request.ECTS,
                request.NationalCredit,
                request.Semester,
                request.Description
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course updated: {CourseId}", course.Id);
            return Result.Success(course.Id, "Ders başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course");
            return Result.Failure<Guid>("Ders güncellenirken bir hata oluştu.", ex.Message);
        }
    }
}

public record AddPrerequisiteCommand(
    Guid CourseId,
    Guid PrerequisiteCourseId
) : IRequest<Result>;

public class AddPrerequisiteCommandValidator : AbstractValidator<AddPrerequisiteCommand>
{
    public AddPrerequisiteCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.PrerequisiteCourseId)
            .NotEmpty().WithMessage("Ön koşul ders ID gereklidir.");

        RuleFor(x => x)
            .Must(x => x.CourseId != x.PrerequisiteCourseId)
            .WithMessage("Bir ders kendisinin ön koşulu olamaz.");
    }
}

public class AddPrerequisiteCommandHandler : IRequestHandler<AddPrerequisiteCommand, Result>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddPrerequisiteCommandHandler> _logger;

    public AddPrerequisiteCommandHandler(
        IRepository<Course> courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddPrerequisiteCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(AddPrerequisiteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
                return Result.Failure("Ders bulunamadı.");

            var prerequisiteCourse = await _courseRepository.GetByIdAsync(request.PrerequisiteCourseId, cancellationToken);
            if (prerequisiteCourse == null)
                return Result.Failure("Ön koşul ders bulunamadı.");

            course.AddPrerequisite(request.PrerequisiteCourseId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Prerequisite added to course: {CourseId} - {PrerequisiteCourseId}",
                request.CourseId, request.PrerequisiteCourseId);

            return Result.Success("Ön koşul başarıyla eklendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding prerequisite");
            return Result.Failure("Ön koşul eklenirken bir hata oluştu.", ex.Message);
        }
    }
}

public record DeleteCourseCommand(Guid Id) : IRequest<Result>;

public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ders ID gereklidir.");
    }
}

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, Result>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<CourseRegistration> _courseRegistrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCourseCommandHandler> _logger;

    public DeleteCourseCommandHandler(
        IRepository<Course> courseRepository,
        IRepository<CourseRegistration> courseRegistrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _courseRegistrationRepository = courseRegistrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (course == null)
            {
                _logger.LogWarning("Course not found for deletion. CourseId: {CourseId}", request.Id);
                return Result.Failure("Ders bulunamadı.");
            }

            // Check if course has active registrations
            var activeRegistrations = await _courseRegistrationRepository.FindAsync(
                cr => cr.CourseId == request.Id && cr.Status == Domain.Enums.CourseRegistrationStatus.Active,
                cancellationToken);

            if (activeRegistrations.Any())
            {
                return Result.Failure("Aktif kayıtlı öğrencisi olan ders silinemez. Önce dersi pasif hale getirin.");
            }

            // Soft delete
            course.Deactivate();
            course.Delete();

            await _courseRepository.UpdateAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course deleted successfully. CourseId: {CourseId}", request.Id);

            return Result.Success("Ders başarıyla silindi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting course. CourseId: {CourseId}", request.Id);
            return Result.Failure("Ders silinirken bir hata oluştu.", ex.Message);
        }
    }
}