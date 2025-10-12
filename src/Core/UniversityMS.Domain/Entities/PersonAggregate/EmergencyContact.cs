using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.PersonAggregate;

public class EmergencyContact : BaseEntity
{
    public string FullName { get; private set; }
    public string Relationship { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public PhoneNumber? AlternativePhone { get; private set; }

    private EmergencyContact() { } // EF Core için

    private EmergencyContact(string fullName, string relationship,
        PhoneNumber phoneNumber, PhoneNumber? alternativePhone = null)
        : base()
    {
        FullName = fullName;
        Relationship = relationship;
        PhoneNumber = phoneNumber;
        AlternativePhone = alternativePhone;
    }

    public static EmergencyContact Create(string fullName, string relationship,
        PhoneNumber phoneNumber, PhoneNumber? alternativePhone = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Acil durum kişisi adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(relationship))
            throw new DomainException("Yakınlık derecesi boş olamaz.");

        return new EmergencyContact(fullName.Trim(), relationship.Trim(),
            phoneNumber, alternativePhone);
    }

    public void Update(string fullName, string relationship,
        PhoneNumber phoneNumber, PhoneNumber? alternativePhone = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Acil durum kişisi adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(relationship))
            throw new DomainException("Yakınlık derecesi boş olamaz.");

        FullName = fullName.Trim();
        Relationship = relationship.Trim();
        PhoneNumber = phoneNumber;
        AlternativePhone = alternativePhone;
    }
}