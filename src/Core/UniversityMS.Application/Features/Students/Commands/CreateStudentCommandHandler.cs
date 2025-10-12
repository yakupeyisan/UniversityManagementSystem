using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.Students.Commands;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<Guid>>
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateStudentCommandHandler> _logger;

    public CreateStudentCommandHandler(
        IRepository<Student> studentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateStudentCommandHandler> logger)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Email ve PhoneNumber value objects oluştur
            var email = Email.Create(request.Email);
            var phoneNumber = PhoneNumber.Create(request.PhoneNumber);

            // Student entity oluştur
            var student = Student.Create(
                request.FirstName,
                request.LastName,
                request.NationalId,
                request.BirthDate,
                request.Gender,
                email,
                phoneNumber,
                request.StudentNumber,
                request.DepartmentId,
                request.EducationLevel
            );

            // Veritabanına ekle
            await _studentRepository.AddAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Student created successfully. StudentId: {StudentId}, StudentNumber: {StudentNumber}",
                student.Id, student.StudentNumber);

            return Result.Success(student.Id, "Öğrenci başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating student. StudentNumber: {StudentNumber}",
                request.StudentNumber);
            return Result.Failure<Guid>("Öğrenci oluşturulurken bir hata oluştu.", ex.Message);
        }
    }
}