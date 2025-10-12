using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, Result<Guid>>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateEnrollmentCommandHandler> _logger;

    public CreateEnrollmentCommandHandler(
        IRepository<Enrollment> enrollmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateEnrollmentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = Enrollment.Create(
                request.StudentId,
                request.AcademicYear,
                request.Semester
            );

            await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Enrollment created: {EnrollmentId} for student {StudentId}",
                enrollment.Id, request.StudentId);

            return Result.Success(enrollment.Id, "Kayıt başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating enrollment");
            return Result.Failure<Guid>("Kayıt oluşturulurken bir hata oluştu.", ex.Message);
        }
    }
}