using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public class RemoveCourseFromEnrollmentCommandHandler : IRequestHandler<RemoveCourseFromEnrollmentCommand, Result>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveCourseFromEnrollmentCommandHandler> _logger;

    public RemoveCourseFromEnrollmentCommandHandler(
        IRepository<Enrollment> enrollmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemoveCourseFromEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RemoveCourseFromEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            if (enrollment == null)
                return Result.Failure("Kayıt bulunamadı.");

            enrollment.RemoveCourse(request.CourseId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course removed from enrollment: {EnrollmentId} - {CourseId}",
                request.EnrollmentId, request.CourseId);

            return Result.Success("Ders kayıttan başarıyla çıkarıldı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing course from enrollment");
            return Result.Failure("Ders çıkarılırken bir hata oluştu.", ex.Message);
        }
    }
}