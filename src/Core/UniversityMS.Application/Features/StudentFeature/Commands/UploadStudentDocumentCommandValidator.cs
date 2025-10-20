using FluentValidation;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public class UploadStudentDocumentCommandValidator
    : AbstractValidator<UploadStudentDocumentCommand>
{
    public UploadStudentDocumentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEqual(Guid.Empty)
            .WithMessage("Öğrenci ID gerekli");

        RuleFor(x => x.FileContent)
            .NotEmpty()
            .WithMessage("Dosya içeriği boş olamaz");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("Dosya adı gerekli")
            .MaximumLength(255)
            .WithMessage("Dosya adı 255 karakteri geçemez");

        RuleFor(x => x.DocumentType)
            .NotEmpty()
            .WithMessage("Belge tipi gerekli")
            .Must(x => new[] { "diploma", "certificate", "transcript", "other" }.Contains(x))
            .WithMessage("Geçersiz belge tipi");

        RuleFor(x => x.MimeType)
            .NotEmpty()
            .WithMessage("MIME type gerekli")
            .Must(x => new[]
            {
                "application/pdf",
                "image/jpeg",
                "image/png",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            }.Contains(x))
            .WithMessage("Desteklenmeyen dosya türü");

        RuleFor(x => x.FileContent.Length)
            .LessThanOrEqualTo(52428800) // 50MB
            .WithMessage("Dosya boyutu 50MB'ı geçemez");
    }
}