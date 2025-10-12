using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Commands;

public class ReviewGradeObjectionCommandHandler : IRequestHandler<ReviewGradeObjectionCommand, Result>
{
    private readonly IRepository<GradeObjection> _objectionRepository;
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReviewGradeObjectionCommandHandler> _logger;

    public ReviewGradeObjectionCommandHandler(
        IRepository<GradeObjection> objectionRepository,
        IRepository<Grade> gradeRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReviewGradeObjectionCommandHandler> logger)
    {
        _objectionRepository = objectionRepository;
        _gradeRepository = gradeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ReviewGradeObjectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var objection = await _objectionRepository.GetByIdAsync(request.ObjectionId, cancellationToken);
            if (objection == null)
                return Result.Failure("İtiraz bulunamadı.");

            if (request.IsApproved)
            {
                if (!request.NewScore.HasValue)
                    return Result.Failure("Yeni not belirtilmelidir.");

                objection.Approve(request.ReviewedBy, request.NewScore.Value, request.ReviewNotes);

                var grade = await _gradeRepository.GetByIdAsync(objection.GradeId, cancellationToken);
                if (grade != null)
                {
                    grade.Update(request.NewScore.Value, request.ReviewNotes);
                    await _gradeRepository.UpdateAsync(grade, cancellationToken);
                }
            }
            else
            {
                objection.Reject(request.ReviewedBy, request.ReviewNotes);
            }

            await _objectionRepository.UpdateAsync(objection, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(request.IsApproved ? "İtiraz onaylandı." : "İtiraz reddedildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reviewing objection");
            return Result.Failure("İtiraz işlenirken hata oluştu.");
        }
    }
}