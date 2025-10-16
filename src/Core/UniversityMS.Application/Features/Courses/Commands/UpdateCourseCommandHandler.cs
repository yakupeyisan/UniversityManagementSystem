using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Courses.Commands;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Result<Guid>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCourseCommandHandler> _logger;

    public UpdateCourseCommandHandler(
        IRepository<Course> courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
            if (course == null)
                return Result<Guid>.Failure("Ders bulunamadı.");

            course.Update(
                request.Name,
                request.TheoreticalHours,
                request.PracticalHours,
                request.ECTS,
                request.NationalCredit,
                request.Semester,
                request.Description
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course updated: {CourseId}", course.Id);
            return Result<Guid>.Success(course.Id, "Ders başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course");
            return Result<Guid>.Failure("Ders güncellenirken bir hata oluştu. Hata: "+ ex.Message);
        }
    }
}