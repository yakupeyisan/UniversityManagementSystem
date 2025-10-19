using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public class UpdateStudentStatusCommandHandler : IRequestHandler<UpdateStudentStatusCommand, Result>
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateStudentStatusCommandHandler> _logger;

    public UpdateStudentStatusCommandHandler(
        IRepository<Student> studentRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateStudentStatusCommandHandler> logger)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateStudentStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);

            if (student == null)
            {
                _logger.LogWarning("Student not found. StudentId: {StudentId}", request.StudentId);
                return Result.Failure("Öğrenci bulunamadı.");
            }

            student.UpdateStatus(request.Status);

            await _studentRepository.UpdateAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Student status updated. StudentId: {StudentId}, Status: {Status}",
                request.StudentId, request.Status);

            return Result.Success("Öğrenci durumu güncellendi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating student status. StudentId: {StudentId}", request.StudentId);
            return Result.Failure("Öğrenci durumu güncellenirken bir hata oluştu.", ex.Message);
        }
    }
}