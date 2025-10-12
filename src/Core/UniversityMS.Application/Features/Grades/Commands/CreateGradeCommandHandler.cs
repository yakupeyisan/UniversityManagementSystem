using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Commands;

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