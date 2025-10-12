using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Commands;

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