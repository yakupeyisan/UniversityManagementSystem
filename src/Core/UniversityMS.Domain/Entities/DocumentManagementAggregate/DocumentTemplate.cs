using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.DocumentManagementAggregate;

/// <summary>
/// Belge Şablonu
/// </summary>
public class DocumentTemplate : AuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public DocumentType Type { get; private set; }
    public string TemplatePath { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int Version { get; private set; }

    private DocumentTemplate() { }

    private DocumentTemplate(string code, string name, DocumentType type, string templatePath)
    {
        Code = code;
        Name = name;
        Type = type;
        TemplatePath = templatePath;
        IsActive = true;
        Version = 1;
    }

    public static DocumentTemplate Create(string code, string name, DocumentType type, string templatePath)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
            throw new DomainException("Kod ve ad boş olamaz.");
        return new DocumentTemplate(code, name, type, templatePath);
    }

    public void Deactivate() => IsActive = false;
    public void UpdateVersion() => Version++;
}