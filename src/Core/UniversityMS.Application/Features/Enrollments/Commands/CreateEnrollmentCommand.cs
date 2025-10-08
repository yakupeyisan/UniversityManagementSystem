using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public record CreateEnrollmentCommand(
    Guid StudentId,
    string AcademicYear,
    int Semester
) : IRequest<Result<Guid>>;

public class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
{
    public CreateEnrollmentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");

        RuleFor(x => x.AcademicYear)
            .NotEmpty().WithMessage("Akademik yıl gereklidir.")
            .Matches(@"^\d{4}-\d{4}$").WithMessage("Akademik yıl formatı: YYYY-YYYY");

        RuleFor(x => x.Semester)
            .InclusiveBetween(1, 2).WithMessage("Dönem 1 (Güz) veya 2 (Bahar) olmalıdır.");
    }
}

public class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, Result<Guid>>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateEnrollmentCommandHandler> _logger;

    public CreateEnrollmentCommandHandler(
        IRepository<Enrollment> enrollmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = Enrollment.Create(
                request.StudentId,
                request.AcademicYear,
                request.Semester
            );

            await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Enrollment created: {EnrollmentId} for student {StudentId}",
                enrollment.Id, request.StudentId);

            return Result.Success(enrollment.Id, "Kayıt başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating enrollment");
            return Result.Failure<Guid>("Kayıt oluşturulurken bir hata oluştu.", ex.Message);
        }
    }
}

public record AddCourseToEnrollmentCommand(
    Guid EnrollmentId,
    Guid CourseId
) : IRequest<Result>;

public class AddCourseToEnrollmentCommandValidator : AbstractValidator<AddCourseToEnrollmentCommand>
{
    public AddCourseToEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID gereklidir.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");
    }
}

public class AddCourseToEnrollmentCommandHandler : IRequestHandler<AddCourseToEnrollmentCommand, Result>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddCourseToEnrollmentCommandHandler> _logger;

    public AddCourseToEnrollmentCommandHandler(
        IRepository<Enrollment> enrollmentRepository,
        IRepository<Course> courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddCourseToEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(AddCourseToEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            if (enrollment == null)
                return Result.Failure("Kayıt bulunamadı.");

            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
                return Result.Failure("Ders bulunamadı.");

            enrollment.AddCourse(request.CourseId, course);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course added to enrollment: {EnrollmentId} - {CourseId}",
                request.EnrollmentId, request.CourseId);

            return Result.Success("Ders kayda başarıyla eklendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding course to enrollment");
            return Result.Failure("Ders eklenirken bir hata oluştu.", ex.Message);
        }
    }
}

public record RejectEnrollmentCommand(
    Guid EnrollmentId,
    Guid AdvisorId,
    string Reason
) : IRequest<Result>;

public class RejectEnrollmentCommandValidator : AbstractValidator<RejectEnrollmentCommand>
{
    public RejectEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID gereklidir.");

        RuleFor(x => x.AdvisorId)
            .NotEmpty().WithMessage("Danışman ID gereklidir.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Red nedeni belirtilmelidir.")
            .MaximumLength(500);
    }
}

public class RejectEnrollmentCommandHandler : IRequestHandler<RejectEnrollmentCommand, Result>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectEnrollmentCommandHandler> _logger;

    public RejectEnrollmentCommandHandler(
        IRepository<Enrollment> enrollmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<RejectEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RejectEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            if (enrollment == null)
                return Result.Failure("Kayıt bulunamadı.");

            enrollment.Reject(request.AdvisorId, request.Reason);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Enrollment rejected: {EnrollmentId} by {AdvisorId}",
                request.EnrollmentId, request.AdvisorId);

            return Result.Success("Kayıt başarıyla reddedildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting enrollment");
            return Result.Failure("Kayıt reddedilirken bir hata oluştu.", ex.Message);
        }
    }
}