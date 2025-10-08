using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
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
public record RemoveCourseFromEnrollmentCommand(
    Guid EnrollmentId,
    Guid CourseId
) : IRequest<Result>;

public class RemoveCourseFromEnrollmentCommandValidator : AbstractValidator<RemoveCourseFromEnrollmentCommand>
{
    public RemoveCourseFromEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID gereklidir.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");
    }
}

public class RemoveCourseFromEnrollmentCommandHandler : IRequestHandler<RemoveCourseFromEnrollmentCommand, Result>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveCourseFromEnrollmentCommandHandler> _logger;

    public RemoveCourseFromEnrollmentCommandHandler(
        IRepository<Enrollment> enrollmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveCourseFromEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RemoveCourseFromEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            if (enrollment == null)
                return Result.Failure("Kayıt bulunamadı.");

            enrollment.RemoveCourse(request.CourseId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course removed from enrollment: {EnrollmentId} - {CourseId}",
                request.EnrollmentId, request.CourseId);

            return Result.Success("Ders kayıttan başarıyla çıkarıldı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing course from enrollment");
            return Result.Failure("Ders çıkarılırken bir hata oluştu.", ex.Message);
        }
    }
}
public record SubmitEnrollmentCommand(Guid EnrollmentId) : IRequest<Result>;

public class SubmitEnrollmentCommandValidator : AbstractValidator<SubmitEnrollmentCommand>
{
    public SubmitEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID gereklidir.");
    }
}

public class SubmitEnrollmentCommandHandler : IRequestHandler<SubmitEnrollmentCommand, Result>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitEnrollmentCommandHandler> _logger;

    public SubmitEnrollmentCommandHandler(
        IRepository<Enrollment> enrollmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<SubmitEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(SubmitEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            if (enrollment == null)
                return Result.Failure("Kayıt bulunamadı.");

            enrollment.Submit();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Enrollment submitted: {EnrollmentId}", request.EnrollmentId);
            return Result.Success("Kayıt danışman onayına gönderildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting enrollment");
            return Result.Failure("Kayıt gönderilirken bir hata oluştu.", ex.Message);
        }
    }
}
public record ApproveEnrollmentCommand(
    Guid EnrollmentId,
    Guid AdvisorId
) : IRequest<Result>;

public class ApproveEnrollmentCommandValidator : AbstractValidator<ApproveEnrollmentCommand>
{
    public ApproveEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Kayıt ID gereklidir.");

        RuleFor(x => x.AdvisorId)
            .NotEmpty().WithMessage("Danışman ID gereklidir.");
    }
}

public class ApproveEnrollmentCommandHandler : IRequestHandler<ApproveEnrollmentCommand, Result>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveEnrollmentCommandHandler> _logger;

    public ApproveEnrollmentCommandHandler(
        IRepository<Enrollment> enrollmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<ApproveEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ApproveEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            if (enrollment == null)
                return Result.Failure("Kayıt bulunamadı.");

            enrollment.Approve(request.AdvisorId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Enrollment approved: {EnrollmentId} by {AdvisorId}",
                request.EnrollmentId, request.AdvisorId);

            return Result.Success("Kayıt başarıyla onaylandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving enrollment");
            return Result.Failure("Kayıt onaylanırken bir hata oluştu.", ex.Message);
        }
    }
}
