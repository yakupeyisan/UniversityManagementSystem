using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.EnrollmentFeature.Commands;

public class AddCourseToEnrollmentCommandHandler : IRequestHandler<AddCourseToEnrollmentCommand, Result>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddCourseToEnrollmentCommandHandler> _logger;

    public AddCourseToEnrollmentCommandHandler(
        IRepository<Enrollment> enrollmentRepository,
        IRepository<Course> courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddCourseToEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(AddCourseToEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            if (enrollment == null)
                return Result.Failure("Kayıt bulunamadı.");

            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
                return Result.Failure("Ders bulunamadı.");

            enrollment.AddCourse(request.CourseId, course);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course added to enrollment: {EnrollmentId} - {CourseId}",
                request.EnrollmentId, request.CourseId);

            return Result.Success("Ders kayda başarıyla eklendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding course to enrollment");
            return Result.Failure("Ders eklenirken bir hata oluştu.", ex.Message);
        }
    }
}