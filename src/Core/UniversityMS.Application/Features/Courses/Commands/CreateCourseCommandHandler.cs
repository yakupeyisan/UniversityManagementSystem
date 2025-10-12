using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Courses.Commands;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Result<Guid>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<Department> _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCourseCommandHandler> _logger;

    public CreateCourseCommandHandler(
        IRepository<Course> courseRepository,
        IRepository<Department> departmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verify department exists
            var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
            if (department == null)
                return Result.Failure<Guid>("Bölüm bulunamadı.");

            // Check if code exists
            var existingCourse = await _courseRepository.FirstOrDefaultAsync(
                c => c.Code == request.Code.Trim().ToUpperInvariant(),
                cancellationToken);

            if (existingCourse != null)
                return Result.Failure<Guid>($"'{request.Code}' kodu zaten kullanımda.");

            var course = Course.Create(
                request.Name,
                request.Code,
                request.DepartmentId,
                request.CourseType,
                request.TheoreticalHours,
                request.PracticalHours,
                request.ECTS,
                request.NationalCredit,
                request.EducationLevel,
                request.Semester,
                request.Description
            );

            await _courseRepository.AddAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Course created: {CourseId} - {CourseName}", course.Id, course.Name);

            return Result.Success(course.Id, "Ders başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course");
            return Result.Failure<Guid>("Ders oluşturulurken bir hata oluştu.", ex.Message);
        }
    }
}