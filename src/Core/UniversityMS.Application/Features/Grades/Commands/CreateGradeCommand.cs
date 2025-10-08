using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Grades.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Commands;

public record CreateGradeCommand(
    Guid CourseRegistrationId,
    Guid StudentId,
    Guid CourseId,
    Guid? InstructorId,
    GradeType GradeType,
    double NumericScore,
    string LetterGrade,
    double GradePoint
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

        RuleFor(x => x.LetterGrade)
            .NotEmpty().WithMessage("Harf notu gereklidir.")
            .MaximumLength(2);

        RuleFor(x => x.GradePoint)
            .InclusiveBetween(0, 4).WithMessage("Not ortalaması 0-4 arasında olmalıdır.");
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
                request.InstructorId,
                request.GradeType,
                request.NumericScore,
                request.LetterGrade,
                request.GradePoint
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

            grade.RuleFor(x => x.LetterGrade)
                .NotEmpty().WithMessage("Harf notu gereklidir.")
                .MaximumLength(2);

            grade.RuleFor(x => x.GradePoint)
                .InclusiveBetween(0, 4).WithMessage("Not ortalaması 0-4 arasında olmalıdır.");
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
                    gradeDto.InstructorId,
                    gradeDto.GradeType,
                    gradeDto.NumericScore,
                    gradeDto.LetterGrade,
                    gradeDto.GradePoint
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