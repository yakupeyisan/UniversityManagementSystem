using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public class ApproveEnrollmentCommandHandler : IRequestHandler<ApproveEnrollmentCommand, Result>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveEnrollmentCommandHandler> _logger;

    public ApproveEnrollmentCommandHandler(
        IRepository<Enrollment> enrollmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<ApproveEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ApproveEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            if (enrollment == null)
                return Result.Failure("Kayıt bulunamadı.");

            enrollment.Approve(request.AdvisorId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Enrollment approved: {EnrollmentId} by {AdvisorId}",
                request.EnrollmentId, request.AdvisorId);

            return Result.Success("Kayıt başarıyla onaylandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving enrollment");
            return Result.Failure("Kayıt onaylanırken bir hata oluştu.", ex.Message);
        }
    }
}