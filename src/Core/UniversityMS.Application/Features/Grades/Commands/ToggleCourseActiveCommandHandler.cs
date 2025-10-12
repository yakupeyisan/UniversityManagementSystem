using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Commands;

public class ToggleCourseActiveCommandHandler : IRequestHandler<ToggleCourseActiveCommand, Result>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ToggleCourseActiveCommandHandler> _logger;

    public ToggleCourseActiveCommandHandler(
        IRepository<Course> courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<ToggleCourseActiveCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ToggleCourseActiveCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

            if (course == null)
            {
                _logger.LogWarning("Course not found. CourseId: {CourseId}", request.CourseId);
                return Result.Failure("Ders bulunamadı.");
            }

            if (request.IsActive)
                course.Activate();
            else
                course.Deactivate();

            await _courseRepository.UpdateAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var status = request.IsActive ? "aktif" : "pasif";
            _logger.LogInformation("Course status toggled. CourseId: {CourseId}, Status: {Status}",
                request.CourseId, status);

            return Result.Success($"Ders {status} hale getirildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling course status. CourseId: {CourseId}", request.CourseId);
            return Result.Failure("Ders durumu değiştirilirken bir hata oluştu.", ex.Message);
        }
    }
}