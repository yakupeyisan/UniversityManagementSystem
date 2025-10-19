using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public class RejectEnrollmentCommandHandler : IRequestHandler<RejectEnrollmentCommand, Result>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectEnrollmentCommandHandler> _logger;

    public RejectEnrollmentCommandHandler(
        IRepository<Enrollment> enrollmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<RejectEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RejectEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            if (enrollment == null)
                return Result.Failure("Kayıt bulunamadı.");

            enrollment.Reject(request.AdvisorId, request.Reason);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Enrollment rejected: {EnrollmentId} by {AdvisorId}",
                request.EnrollmentId, request.AdvisorId);

            return Result.Success("Kayıt başarıyla reddedildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting enrollment");
            return Result.Failure("Kayıt reddedilirken bir hata oluştu.", ex.Message);
        }
    }
}