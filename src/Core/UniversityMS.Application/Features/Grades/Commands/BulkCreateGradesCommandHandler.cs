using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Commands;

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

            return Result<int>.Success(grades.Count, $"{grades.Count} not başarıyla girildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bulk grades");
            return Result<int>.Failure("Notlar girilirken bir hata oluştu. Hata: "+ ex.Message);
        }
    }
}