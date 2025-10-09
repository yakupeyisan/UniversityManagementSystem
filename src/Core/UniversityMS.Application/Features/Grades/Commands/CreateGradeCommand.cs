using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Grades.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Commands;

public record CreateGradeCommand(
    Guid CourseRegistrationId,
    Guid StudentId,
    Guid CourseId,
    GradeType GradeType,
    double NumericScore,
    double Weight,
    Guid? InstructorId = null
) : IRequest<Result<Guid>>;

public class CreateGradeCommandValidator : AbstractValidator<CreateGradeCommand>
{
    public CreateGradeCommandValidator()
    {
        RuleFor(x => x.CourseRegistrationId)
            .NotEmpty().WithMessage("Ders kayıt ID gereklidir.");

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.NumericScore)
            .InclusiveBetween(0, 100).WithMessage("Not 0-100 arasında olmalıdır.");

        RuleFor(x => x.Weight)
            .InclusiveBetween(0, 1).WithMessage("Ağırlık 0-1 arasında olmalıdır.");
    }
}

public class CreateGradeCommandHandler : IRequestHandler<CreateGradeCommand, Result<Guid>>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateGradeCommandHandler> _logger;

    public CreateGradeCommandHandler(
        IRepository<Grade> gradeRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateGradeCommandHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateGradeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var grade = Grade.Create(
                request.CourseRegistrationId,
                request.StudentId,
                request.CourseId,
                request.GradeType,
                request.NumericScore,
                request.Weight,
                request.InstructorId
            );

            await _gradeRepository.AddAsync(grade, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Grade created: {GradeId} for student {StudentId}",
                grade.Id, request.StudentId);

            return Result.Success(grade.Id, "Not başarıyla girildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating grade");
            return Result.Failure<Guid>("Not girilirken bir hata oluştu.", ex.Message);
        }
    }
}

public record BulkCreateGradesCommand(
    List<GradeDto> Grades
) : IRequest<Result<int>>;

public class BulkCreateGradesCommandValidator : AbstractValidator<BulkCreateGradesCommand>
{
    public BulkCreateGradesCommandValidator()
    {
        RuleFor(x => x.Grades)
            .NotEmpty().WithMessage("En az bir not girişi yapılmalıdır.");

        RuleForEach(x => x.Grades).ChildRules(grade =>
        {
            grade.RuleFor(x => x.CourseRegistrationId)
                .NotEmpty().WithMessage("Ders kayıt ID gereklidir.");

            grade.RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Öğrenci ID gereklidir.");

            grade.RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Ders ID gereklidir.");

            grade.RuleFor(x => x.NumericScore)
                .InclusiveBetween(0, 100).WithMessage("Not 0-100 arasında olmalıdır.");

            grade.RuleFor(x => x.Weight)
                .InclusiveBetween(0, 1).WithMessage("Ağırlık 0-1 arasında olmalıdır.");
        });
    }
}

public class BulkCreateGradesCommandHandler : IRequestHandler<BulkCreateGradesCommand, Result<int>>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BulkCreateGradesCommandHandler> _logger;

    public BulkCreateGradesCommandHandler(
        IRepository<Grade> gradeRepository,
        IUnitOfWork unitOfWork,
        ILogger<BulkCreateGradesCommandHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(BulkCreateGradesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var grades = new List<Grade>();

            foreach (var gradeDto in request.Grades)
            {
                var grade = Grade.Create(
                    gradeDto.CourseRegistrationId,
                    gradeDto.StudentId,
                    gradeDto.CourseId,
                    gradeDto.GradeType,
                    gradeDto.NumericScore,
                    gradeDto.Weight,
                    gradeDto.InstructorId
                );
                grades.Add(grade);
            }

            foreach (var grade in grades)
            {
                await _gradeRepository.AddAsync(grade, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Bulk grades created: {Count} grades", grades.Count);

            return Result.Success(grades.Count, $"{grades.Count} not başarıyla girildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bulk grades");
            return Result.Failure<int>("Notlar girilirken bir hata oluştu.", ex.Message);
        }
    }
}

public record UpdateGradeCommand(
    Guid Id,
    double NumericScore,
    string? Notes = null
) : IRequest<Result>;

public class UpdateGradeCommandValidator : AbstractValidator<UpdateGradeCommand>
{
    public UpdateGradeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Not ID gereklidir.");

        RuleFor(x => x.NumericScore)
            .InclusiveBetween(0, 100).WithMessage("Not 0-100 arasında olmalıdır.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olabilir.");
    }
}

public class UpdateGradeCommandHandler : IRequestHandler<UpdateGradeCommand, Result>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateGradeCommandHandler> _logger;

    public UpdateGradeCommandHandler(
        IRepository<Grade> gradeRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateGradeCommandHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateGradeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var grade = await _gradeRepository.GetByIdAsync(request.Id, cancellationToken);

            if (grade == null)
            {
                _logger.LogWarning("Grade not found for update. GradeId: {GradeId}", request.Id);
                return Result.Failure("Not kaydı bulunamadı.");
            }

            // Update grade
            grade.Update(request.NumericScore, request.Notes);

            await _gradeRepository.UpdateAsync(grade, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Grade updated successfully. GradeId: {GradeId}, NewScore: {Score}",
                request.Id, request.NumericScore);

            return Result.Success("Not başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating grade. GradeId: {GradeId}", request.Id);
            return Result.Failure("Not güncellenirken bir hata oluştu.", ex.Message);
        }
    }
}

public record DeleteGradeCommand(Guid Id) : IRequest<Result>;

public class DeleteGradeCommandValidator : AbstractValidator<DeleteGradeCommand>
{
    public DeleteGradeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Not ID gereklidir.");
    }
}

public class DeleteGradeCommandHandler : IRequestHandler<DeleteGradeCommand, Result>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteGradeCommandHandler> _logger;

    public DeleteGradeCommandHandler(
        IRepository<Grade> gradeRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteGradeCommandHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteGradeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var grade = await _gradeRepository.GetByIdAsync(request.Id, cancellationToken);

            if (grade == null)
            {
                _logger.LogWarning("Grade not found for deletion. GradeId: {GradeId}", request.Id);
                return Result.Failure("Not kaydı bulunamadı.");
            }

            await _gradeRepository.DeleteAsync(grade, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Grade deleted successfully. GradeId: {GradeId}", request.Id);

            return Result.Success("Not başarıyla silindi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting grade. GradeId: {GradeId}", request.Id);
            return Result.Failure("Not silinirken bir hata oluştu.", ex.Message);
        }
    }
}

public record CreateGradeObjectionCommand(
    Guid GradeId,
    Guid StudentId,
    Guid CourseId,
    string Reason
) : IRequest<Result<Guid>>;

public class CreateGradeObjectionCommandValidator : AbstractValidator<CreateGradeObjectionCommand>
{
    public CreateGradeObjectionCommandValidator()
    {
        RuleFor(x => x.GradeId)
            .NotEmpty().WithMessage("Not ID gereklidir.");

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("İtiraz nedeni belirtilmelidir.")
            .MinimumLength(10).WithMessage("İtiraz nedeni en az 10 karakter olmalıdır.")
            .MaximumLength(1000).WithMessage("İtiraz nedeni en fazla 1000 karakter olabilir.");
    }
}

public class CreateGradeObjectionCommandHandler : IRequestHandler<CreateGradeObjectionCommand, Result<Guid>>
{
    private readonly IRepository<Grade> _gradeRepository;
    // private readonly IRepository<GradeObjection> _objectionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateGradeObjectionCommandHandler> _logger;

    public CreateGradeObjectionCommandHandler(
        IRepository<Grade> gradeRepository,
        // IRepository<GradeObjection> objectionRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateGradeObjectionCommandHandler> logger)
    {
        _gradeRepository = gradeRepository;
        // _objectionRepository = objectionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateGradeObjectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verify grade exists
            var grade = await _gradeRepository.GetByIdAsync(request.GradeId, cancellationToken);
            if (grade == null)
                return Result.Failure<Guid>("Not kaydı bulunamadı.");

            // Verify student owns the grade
            if (grade.StudentId != request.StudentId)
                return Result.Failure<Guid>("Bu nota itiraz etme yetkiniz yok.");

            /*
            // Check if objection already exists
            var existingObjection = await _objectionRepository.FirstOrDefaultAsync(
                o => o.GradeId == request.GradeId && o.Status == ObjectionStatus.Pending,
                cancellationToken);

            if (existingObjection != null)
                return Result.Failure<Guid>("Bu not için zaten bekleyen bir itiraz var.");

            // Create objection
            var objection = GradeObjection.Create(
                request.GradeId,
                request.StudentId,
                request.CourseId,
                request.Reason
            );

            await _objectionRepository.AddAsync(objection, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Grade objection created. ObjectionId: {ObjectionId}, GradeId: {GradeId}",
                objection.Id, request.GradeId);

            return Result.Success(objection.Id, "Not itirazı başarıyla oluşturuldu.");
            */

            // Placeholder until GradeObjection entity is added
            _logger.LogInformation("Grade objection request received for GradeId: {GradeId}", request.GradeId);
            return Result.Success(Guid.NewGuid(), "Not itirazı alındı. (Geçici yanıt)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating grade objection");
            return Result.Failure<Guid>("Not itirazı oluşturulurken bir hata oluştu.", ex.Message);
        }
    }
}

public record ReviewGradeObjectionCommand(
    Guid ObjectionId,
    Guid ReviewedBy,
    bool IsApproved,
    double? NewScore = null,
    string? ReviewNotes = null
) : IRequest<Result>;

public class ReviewGradeObjectionCommandValidator : AbstractValidator<ReviewGradeObjectionCommand>
{
    public ReviewGradeObjectionCommandValidator()
    {
        RuleFor(x => x.ObjectionId)
            .NotEmpty().WithMessage("İtiraz ID gereklidir.");

        RuleFor(x => x.ReviewedBy)
            .NotEmpty().WithMessage("İncelemeyi yapan kişi belirtilmelidir.");

        RuleFor(x => x.NewScore)
            .InclusiveBetween(0, 100)
            .When(x => x.IsApproved && x.NewScore.HasValue)
            .WithMessage("Yeni not 0-100 arasında olmalıdır.");

        RuleFor(x => x.NewScore)
            .NotNull()
            .When(x => x.IsApproved)
            .WithMessage("İtiraz onaylanıyorsa yeni not girilmelidir.");

        RuleFor(x => x.ReviewNotes)
            .MaximumLength(1000).WithMessage("İnceleme notları en fazla 1000 karakter olabilir.");
    }
}

public class ReviewGradeObjectionCommandHandler : IRequestHandler<ReviewGradeObjectionCommand, Result>
{
    // private readonly IRepository<GradeObjection> _objectionRepository;
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReviewGradeObjectionCommandHandler> _logger;

    public ReviewGradeObjectionCommandHandler(
        // IRepository<GradeObjection> objectionRepository,
        IRepository<Grade> gradeRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReviewGradeObjectionCommandHandler> logger)
    {
        // _objectionRepository = objectionRepository;
        _gradeRepository = gradeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ReviewGradeObjectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            /*
            var objection = await _objectionRepository.GetByIdAsync(request.ObjectionId, cancellationToken);
            if (objection == null)
                return Result.Failure("İtiraz kaydı bulunamadı.");

            if (request.IsApproved)
            {
                // Approve objection and update grade
                objection.Approve(request.ReviewedBy, request.NewScore!.Value, request.ReviewNotes);

                var grade = await _gradeRepository.GetByIdAsync(objection.GradeId, cancellationToken);
                if (grade != null)
                {
                    objection.OldScore = grade.NumericScore;
                    grade.Update(request.NewScore!.Value, $"İtiraz sonucu güncellendi. {request.ReviewNotes}");
                    await _gradeRepository.UpdateAsync(grade, cancellationToken);
                }
            }
            else
            {
                // Reject objection
                objection.Reject(request.ReviewedBy, request.ReviewNotes);
            }

            await _objectionRepository.UpdateAsync(objection, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var action = request.IsApproved ? "onaylandı" : "reddedildi";
            _logger.LogInformation("Grade objection {Action}. ObjectionId: {ObjectionId}",
                action, request.ObjectionId);

            return Result.Success($"Not itirazı {action}.");
            */

            // Placeholder
            var action = request.IsApproved ? "onaylandı" : "reddedildi";
            _logger.LogInformation("Grade objection review: {ObjectionId} - {Action}", request.ObjectionId, action);
            return Result.Success($"Not itirazı {action}. (Geçici yanıt)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reviewing grade objection");
            return Result.Failure("Not itirazı incelenirken bir hata oluştu.", ex.Message);
        }
    }
}

public record ToggleCourseActiveCommand(
    Guid CourseId,
    bool IsActive
) : IRequest<Result>;

public class ToggleCourseActiveCommandValidator : AbstractValidator<ToggleCourseActiveCommand>
{
    public ToggleCourseActiveCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");
    }
}

public class ToggleCourseActiveCommandHandler : IRequestHandler<ToggleCourseActiveCommand, Result>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ToggleCourseActiveCommandHandler> _logger;

    public ToggleCourseActiveCommandHandler(
        IRepository<Course> courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<ToggleCourseActiveCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ToggleCourseActiveCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

            if (course == null)
            {
                _logger.LogWarning("Course not found. CourseId: {CourseId}", request.CourseId);
                return Result.Failure("Ders bulunamadı.");
            }

            if (request.IsActive)
                course.Activate();
            else
                course.Deactivate();

            await _courseRepository.UpdateAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var status = request.IsActive ? "aktif" : "pasif";
            _logger.LogInformation("Course status toggled. CourseId: {CourseId}, Status: {Status}",
                request.CourseId, status);

            return Result.Success($"Ders {status} hale getirildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling course status. CourseId: {CourseId}", request.CourseId);
            return Result.Failure("Ders durumu değiştirilirken bir hata oluştu.", ex.Message);
        }
    }
}


public record RemovePrerequisiteCommand(
    Guid CourseId,
    Guid PrerequisiteCourseId
) : IRequest<Result>;

public class RemovePrerequisiteCommandValidator : AbstractValidator<RemovePrerequisiteCommand>
{
    public RemovePrerequisiteCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Ders ID gereklidir.");

        RuleFor(x => x.PrerequisiteCourseId)
            .NotEmpty().WithMessage("Ön koşul ders ID gereklidir.");
    }
}

public class RemovePrerequisiteCommandHandler : IRequestHandler<RemovePrerequisiteCommand, Result>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemovePrerequisiteCommandHandler> _logger;

    public RemovePrerequisiteCommandHandler(
        IRepository<Course> courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemovePrerequisiteCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RemovePrerequisiteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

            if (course == null)
            {
                _logger.LogWarning("Course not found. CourseId: {CourseId}", request.CourseId);
                return Result.Failure("Ders bulunamadı.");
            }

            course.RemovePrerequisite(request.PrerequisiteCourseId);

            await _courseRepository.UpdateAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Prerequisite removed from course. CourseId: {CourseId}, PrerequisiteId: {PrerequisiteId}",
                request.CourseId, request.PrerequisiteCourseId);

            return Result.Success("Ön koşul kaldırıldı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing prerequisite. CourseId: {CourseId}", request.CourseId);
            return Result.Failure("Ön koşul kaldırılırken bir hata oluştu.", ex.Message);
        }
    }
}