using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
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

public record DeleteStudentCommand(Guid Id) : IRequest<Result>;

public class DeleteStudentCommandValidator : AbstractValidator<DeleteStudentCommand>
{
    public DeleteStudentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");
    }
}

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

public record UpdateStudentStatusCommand(
    Guid StudentId,
    StudentStatus Status
) : IRequest<Result>;

public class UpdateStudentStatusCommandValidator : AbstractValidator<UpdateStudentStatusCommand>
{
    public UpdateStudentStatusCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Geçersiz durum.");
    }
}

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


public record FreezeStudentCommand(Guid StudentId) : IRequest<Result>;

public class FreezeStudentCommandValidator : AbstractValidator<FreezeStudentCommand>
{
    public FreezeStudentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");
    }
}

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

public record GraduateStudentCommand(Guid StudentId) : IRequest<Result>;

public class GraduateStudentCommandValidator : AbstractValidator<GraduateStudentCommand>
{
    public GraduateStudentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öğrenci ID gereklidir.");
    }
}

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
