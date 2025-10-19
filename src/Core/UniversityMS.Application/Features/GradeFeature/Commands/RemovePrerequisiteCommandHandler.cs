using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public class RemovePrerequisiteCommandHandler : IRequestHandler<RemovePrerequisiteCommand, Result>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemovePrerequisiteCommandHandler> _logger;

    public RemovePrerequisiteCommandHandler(
        IRepository<Course> courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemovePrerequisiteCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RemovePrerequisiteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

            if (course == null)
            {
                _logger.LogWarning("Course not found. CourseId: {CourseId}", request.CourseId);
                return Result.Failure("Ders bulunamadı.");
            }

            course.RemovePrerequisite(request.PrerequisiteCourseId);

            await _courseRepository.UpdateAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Prerequisite removed from course. CourseId: {CourseId}, PrerequisiteId: {PrerequisiteId}",
                request.CourseId, request.PrerequisiteCourseId);

            return Result.Success("Ön koşul kaldırıldı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing prerequisite. CourseId: {CourseId}", request.CourseId);
            return Result.Failure("Ön koşul kaldırılırken bir hata oluştu.", ex.Message);
        }
    }
}