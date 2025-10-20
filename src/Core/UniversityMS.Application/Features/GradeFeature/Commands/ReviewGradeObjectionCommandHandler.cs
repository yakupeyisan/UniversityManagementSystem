using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public class ReviewGradeObjectionCommandHandler : IRequestHandler<ReviewGradeObjectionCommand, Result>
{
    private readonly IRepository<GradeObjection> _objectionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReviewGradeObjectionCommandHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;  // ✅ EKLE: Current user'ı almak için

    public ReviewGradeObjectionCommandHandler(
        IRepository<GradeObjection> objectionRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReviewGradeObjectionCommandHandler> logger,
        IHttpContextAccessor httpContextAccessor)  // ✅ EKLE: DI'ye ekle
    {
        _objectionRepository = objectionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(ReviewGradeObjectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var objection = await _objectionRepository.GetByIdAsync(request.ObjectionId, cancellationToken);
            if (objection == null)
                return Result.Failure("İtiraz bulunamadı.");

            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var reviewedBy))
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

