using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.Students.Commands;

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Result>
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateStudentCommandHandler> _logger;

    public UpdateStudentCommandHandler(
        IRepository<Student> studentRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateStudentCommandHandler> logger)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);

            if (student == null)
            {
                throw new EntityNotFoundException(nameof(Student), request.Id);
            }

            var phoneNumber = PhoneNumber.Create(request.PhoneNumber);
            student.UpdateBasicInfo(request.FirstName, request.LastName, phoneNumber);

            await _studentRepository.UpdateAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Student updated successfully. StudentId: {StudentId}", student.Id);

            return Result.Success("Öğrenci bilgileri güncellendi.");
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogWarning(ex, "Student not found. StudentId: {StudentId}", request.Id);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating student. StudentId: {StudentId}", request.Id);
            return Result.Failure("Öğrenci güncellenirken bir hata oluştu.", ex.Message);
        }
    }
}