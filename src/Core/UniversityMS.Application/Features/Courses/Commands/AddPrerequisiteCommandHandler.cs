using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Courses.Commands;

public class AddPrerequisiteCommandHandler : IRequestHandler<AddPrerequisiteCommand, Result>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddPrerequisiteCommandHandler> _logger;

    public AddPrerequisiteCommandHandler(
        IRepository<Course> courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddPrerequisiteCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(AddPrerequisiteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
                return Result.Failure("Ders bulunamadı.");

            var prerequisiteCourse = await _courseRepository.GetByIdAsync(request.PrerequisiteCourseId, cancellationToken);
            if (prerequisiteCourse == null)
                return Result.Failure("Ön koşul ders bulunamadı.");

            course.AddPrerequisite(request.PrerequisiteCourseId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Prerequisite added to course: {CourseId} - {PrerequisiteCourseId}",
                request.CourseId, request.PrerequisiteCourseId);

            return Result.Success("Ön koşul başarıyla eklendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding prerequisite");
            return Result.Failure("Ön koşul eklenirken bir hata oluştu.", ex.Message);
        }
    }
}