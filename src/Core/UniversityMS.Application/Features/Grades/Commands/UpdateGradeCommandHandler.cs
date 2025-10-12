using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Commands;

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
            var grade = await _gradeRepository.GetByIdAsync(request.GradeId, cancellationToken);
            if (grade == null)
                return Result.Failure("Not bulunamadı.");

            grade.Update(request.NumericScore, request.Notes);
            await _gradeRepository.UpdateAsync(grade, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Not güncellendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating grade");
            return Result.Failure("Not güncellenirken hata oluştu.");
        }
    }
}