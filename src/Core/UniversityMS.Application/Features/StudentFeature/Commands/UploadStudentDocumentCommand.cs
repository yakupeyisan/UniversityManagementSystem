using MediatR;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public class UploadStudentDocumentCommand : IRequest<string>
{
    public Guid StudentId { get; set; }
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty; // diploma, certificate, vb.
    public string MimeType { get; set; } = string.Empty;
}