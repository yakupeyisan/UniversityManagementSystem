using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Commands;

public class SubmitGradeCommandHandler : IRequestHandler<SubmitGradeCommand, Result<Guid>>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IRepository<CourseRegistration> _courseRegistrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitGradeCommandHandler> _logger;

    public SubmitGradeCommandHandler(
        IRepository<Grade> gradeRepository,
        IRepository<CourseRegistration> courseRegistrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<SubmitGradeCommandHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _courseRegistrationRepository = courseRegistrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(SubmitGradeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verify course registration exists
            var courseRegistration = await _courseRegistrationRepository.GetByIdAsync(
                request.CourseRegistrationId, cancellationToken);

            if (courseRegistration == null)
                return Result.Failure<Guid>("Ders kaydı bulunamadı.");

            // Check if grade already exists for this type
            var existingGrade = await _gradeRepository.FirstOrDefaultAsync(
                g => g.CourseRegistrationId == request.CourseRegistrationId &&
                     g.GradeType == request.GradeType,
                cancellationToken);

            if (existingGrade != null)
                return Result.Failure<Guid>("Bu sınav türü için not zaten girilmiş.");

            var grade = Grade.Create(
                request.CourseRegistrationId,
                request.StudentId,
                request.CourseId,
                request.GradeType,
                request.NumericScore,
                request.Weight,
                request.InstructorId
            );

            if (!string.IsNullOrWhiteSpace(request.Notes))
                grade.Update(request.NumericScore, request.Notes);

            await _gradeRepository.AddAsync(grade, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Grade submitted: {GradeId} for student {StudentId}",
                grade.Id, request.StudentId);

            return Result.Success(grade.Id, "Not başarıyla kaydedildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting grade");
            return Result.Failure<Guid>("Not kaydedilirken bir hata oluştu.", ex.Message);
        }
    }
}