using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Courses.Commands;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, Result>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<CourseRegistration> _courseRegistrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCourseCommandHandler> _logger;

    public DeleteCourseCommandHandler(
        IRepository<Course> courseRepository,
        IRepository<CourseRegistration> courseRegistrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _courseRegistrationRepository = courseRegistrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (course == null)
            {
                _logger.LogWarning("Course not found for deletion. CourseId: {CourseId}", request.Id);
                return Result.Failure("Ders bulunamadı.");
            }

            // Check if course has active registrations
            var activeRegistrations = await _courseRegistrationRepository.FindAsync(
                cr => cr.CourseId == request.Id && cr.Status == Domain.Enums.CourseRegistrationStatus.Active,
                cancellationToken);

            if (activeRegistrations.Any())
            {
                return Result.Failure("Aktif kayıtlı öğrencisi olan ders silinemez. Önce dersi pasif hale getirin.");
            }

            // Soft delete
            course.Deactivate();
            course.Delete();

            await _courseRepository.UpdateAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course deleted successfully. CourseId: {CourseId}", request.Id);

            return Result.Success("Ders başarıyla silindi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting course. CourseId: {CourseId}", request.Id);
            return Result.Failure("Ders silinirken bir hata oluştu.", ex.Message);
        }
    }
}