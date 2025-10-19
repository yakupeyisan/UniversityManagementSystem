using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public class FreezeStudentCommandHandler : IRequestHandler<FreezeStudentCommand, Result>
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FreezeStudentCommandHandler> _logger;

    public FreezeStudentCommandHandler(
        IRepository<Student> studentRepository,
        IUnitOfWork unitOfWork,
        ILogger<FreezeStudentCommandHandler> logger)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(FreezeStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);

            if (student == null)
            {
                _logger.LogWarning("Student not found. StudentId: {StudentId}", request.StudentId);
                return Result.Failure("Öğrenci bulunamadı.");
            }

            student.Freeze();

            await _studentRepository.UpdateAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Student frozen. StudentId: {StudentId}", request.StudentId);

            return Result.Success("Öğrenci kaydı donduruldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error freezing student. StudentId: {StudentId}", request.StudentId);
            return Result.Failure("Öğrenci dondurulamadı.", ex.Message);
        }
    }
}