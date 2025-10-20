using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public class ReviewGradeObjectionCommandHandler : IRequestHandler<ReviewGradeObjectionCommand, Result>
{
    private readonly IRepository<GradeObjection> _objectionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReviewGradeObjectionCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public ReviewGradeObjectionCommandHandler(
        IRepository<GradeObjection> objectionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<ReviewGradeObjectionCommandHandler> logger)  
    {
        _objectionRepository = objectionRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result> Handle(ReviewGradeObjectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var objection = await _objectionRepository.GetByIdAsync(request.ObjectionId, cancellationToken);
            if (objection == null)
                return Result.Failure("İtiraz bulunamadı.");

            var reviewedBy = _currentUserService.UserId;
            if (reviewedBy == null)
                return Result.Failure("Kullanıcı bilgisi alınamadı.");

            if (request.IsApproved)
            {
                if (!request.NewScore.HasValue)
                    return Result.Failure("Onay için yeni not belirtilmelidir.");

                if (request.NewScore.Value < 0 || request.NewScore.Value > 100)
                    return Result.Failure("Yeni not 0-100 arasında olmalıdır.");

                objection.Approve(reviewedBy, request.NewScore.Value, request.ReviewNotes);
            }
            else
            {
                objection.Reject(reviewedBy, request.ReviewNotes);
            }

            await _objectionRepository.UpdateAsync(objection, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Grade objection reviewed. ObjectionId: {ObjectionId}, IsApproved: {IsApproved}, ReviewedBy: {ReviewedBy}",
                request.ObjectionId,
                request.IsApproved,
                reviewedBy);

            return Result.Success("İtiraz başarıyla incelendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reviewing grade objection");
            return Result.Failure("İtiraz incelenirken bir hata oluştu.", ex.Message);
        }
    }
}

