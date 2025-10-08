using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.Students.Commands;

public record CreateStudentCommand(
    string FirstName,
    string LastName,
    string NationalId,
    DateTime BirthDate,
    Gender Gender,
    string Email,
    string PhoneNumber,
    string StudentNumber,
    Guid DepartmentId,
    EducationLevel EducationLevel
) : IRequest<Result<Guid>>;

public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz.")
            .MaximumLength(100).WithMessage("Ad en fazla 100 karakter olabilir.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad boş olamaz.")
            .MaximumLength(100).WithMessage("Soyad en fazla 100 karakter olabilir.");

        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("TC Kimlik No boş olamaz.")
            .Length(11).WithMessage("TC Kimlik No 11 haneli olmalıdır.")
            .Matches("^[0-9]+$").WithMessage("TC Kimlik No sadece rakamlardan oluşmalıdır.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Doğum tarihi boş olamaz.")
            .LessThan(DateTime.Today.AddYears(-15)).WithMessage("Öğrenci en az 15 yaşında olmalıdır.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
            .Matches("^[0-9]{10,11}$").WithMessage("Geçerli bir telefon numarası giriniz.");

        RuleFor(x => x.StudentNumber)
            .NotEmpty().WithMessage("Öğrenci numarası boş olamaz.")
            .MaximumLength(20).WithMessage("Öğrenci numarası en fazla 20 karakter olabilir.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Bölüm seçilmelidir.");
    }
}

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

public record UpdateStudentCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string PhoneNumber
) : IRequest<Result>;

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
