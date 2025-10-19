using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public class ReviewGradeObjectionCommandHandler : IRequestHandler<ReviewGradeObjectionCommand, Result>
{
    private readonly IRepository<GradeObjection> _objectionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReviewGradeObjectionCommandHandler> _logger;

    public ReviewGradeObjectionCommandHandler(
        IRepository<GradeObjection> objectionRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReviewGradeObjectionCommandHandler> logger)
    {
        _objectionRepository = objectionRepository;
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

            // GradeObjection.Review method çağrı - eğer yoksa update properties
            // Kontrol et: objection.Review(); method var mı?
            // Yoksa manual update yap:

            if (request.IsApproved)
            {
                objection.Approve(request.ReviewNotes ?? string.Empty);
            }
            else
            {
                objection.Reject(request.ReviewNotes ?? string.Empty);
            }

            await _objectionRepository.UpdateAsync(objection, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Grade objection reviewed. ObjectionId: {ObjectionId}", request.ObjectionId);
            return Result.Success("İtiraz başarıyla incelendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reviewing grade objection");
            return Result.Failure("İtiraz incelenirken bir hata oluştu.");
        }
    }
}
