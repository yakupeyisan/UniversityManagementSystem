using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public class SubmitEnrollmentCommandHandler : IRequestHandler<SubmitEnrollmentCommand, Result>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitEnrollmentCommandHandler> _logger;

    public SubmitEnrollmentCommandHandler(
        IRepository<Enrollment> enrollmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<SubmitEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(SubmitEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            if (enrollment == null)
                return Result.Failure("Kayıt bulunamadı.");

            enrollment.Submit();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Enrollment submitted: {EnrollmentId}", request.EnrollmentId);
            return Result.Success("Kayıt danışman onayına gönderildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting enrollment");
            return Result.Failure("Kayıt gönderilirken bir hata oluştu.", ex.Message);
        }
    }
}