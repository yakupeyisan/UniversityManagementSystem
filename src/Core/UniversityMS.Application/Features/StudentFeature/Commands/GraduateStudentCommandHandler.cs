using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public class GraduateStudentCommandHandler : IRequestHandler<GraduateStudentCommand, Result>
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GraduateStudentCommandHandler> _logger;

    public GraduateStudentCommandHandler(
        IRepository<Student> studentRepository,
        IUnitOfWork unitOfWork,
        ILogger<GraduateStudentCommandHandler> logger)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(GraduateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);

            if (student == null)
            {
                _logger.LogWarning("Student not found. StudentId: {StudentId}", request.StudentId);
                return Result.Failure("Öğrenci bulunamadı.");
            }

            student.Graduate();

            await _studentRepository.UpdateAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Student graduated. StudentId: {StudentId}", request.StudentId);

            return Result.Success("Öğrenci mezun edildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error graduating student. StudentId: {StudentId}", request.StudentId);
            return Result.Failure("Öğrenci mezun edilirken bir hata oluştu.", ex.Message);
        }
    }
}