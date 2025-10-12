using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Students.Commands;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Result>
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteStudentCommandHandler> _logger;

    public DeleteStudentCommandHandler(
        IRepository<Student> studentRepository,
        IRepository<Enrollment> enrollmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteStudentCommandHandler> logger)
    {
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);

            if (student == null)
            {
                _logger.LogWarning("Student not found for deletion. StudentId: {StudentId}", request.Id);
                return Result.Failure("Öğrenci bulunamadı.");
            }

            // Check if student has active enrollments
            var activeEnrollments = await _enrollmentRepository.FindAsync(
                e => e.StudentId == request.Id && e.Status == Domain.Enums.EnrollmentStatus.Approved,
                cancellationToken);

            if (activeEnrollments.Any())
            {
                return Result.Failure("Aktif ders kaydı olan öğrenci silinemez. Önce kayıtları pasif hale getirin.");
            }

            // Soft delete
            student.Delete();

            await _studentRepository.UpdateAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Student deleted successfully. StudentId: {StudentId}", request.Id);

            return Result.Success("Öğrenci başarıyla silindi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting student. StudentId: {StudentId}", request.Id);
            return Result.Failure("Öğrenci silinirken bir hata oluştu.", ex.Message);
        }
    }
}