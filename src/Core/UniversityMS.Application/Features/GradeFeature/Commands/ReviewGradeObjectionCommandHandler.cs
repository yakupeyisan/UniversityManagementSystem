using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

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

            objection.Review(request.IsApproved, request.ReviewNotes);

            await _objectionRepository.UpdateAsync(objection, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Grade objection reviewed. ObjectionId: {ObjectionId}, IsApproved: {IsApproved}",
                request.ObjectionId, request.IsApproved);
            return Result.Success("İtiraz başarıyla incelendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reviewing grade objection");
            return Result.Failure("İtiraz incelenirken bir hata oluştu.", ex.Message);
        }
    }
}