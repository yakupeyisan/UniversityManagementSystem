using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public class UploadStudentDocumentCommandHandler
    : IRequestHandler<UploadStudentDocumentCommand, string>
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<UploadStudentDocumentCommandHandler> _logger;

    public UploadStudentDocumentCommandHandler(
        IRepository<Student> studentRepository,
        IFileStorageService fileStorageService,
        ILogger<UploadStudentDocumentCommandHandler> logger)
    {
        _studentRepository = studentRepository;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<string> Handle(UploadStudentDocumentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Öğrenci kontrolü
        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student == null)
            throw new NotFoundException(nameof(Student), request.StudentId.ToString());

        // 2. Klasör yolunu oluştur (students/{studentId}/documents/{documentType}/)
        var folderPath = Path.Combine(
            "students",
            request.StudentId.ToString(),
            "documents",
            request.DocumentType);

        // 3. Dosyayı kaydet
        var savedPath = await _fileStorageService.SaveFileAsync(
            fileContent: request.FileContent,
            fileName: request.FileName,
            folderPath: folderPath,
            mimeType: request.MimeType,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Student document uploaded - StudentId: {StudentId}, " +
            "DocumentType: {DocumentType}, SavedPath: {SavedPath}",
            request.StudentId,
            request.DocumentType,
            savedPath);

        return savedPath;
    }
}