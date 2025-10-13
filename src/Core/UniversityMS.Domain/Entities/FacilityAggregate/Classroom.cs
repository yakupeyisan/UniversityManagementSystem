using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.FacilityAggregate;

/// <summary>
/// Derslik/Sınıf Entity
/// </summary>
public class Classroom : AuditableEntity, ISoftDelete
{
    public string Code { get; private set; } // Örn: A-101
    public string Name { get; private set; }
    public string? Building { get; private set; }
    public int Floor { get; private set; }
    public int Capacity { get; private set; }
    public ClassroomType Type { get; private set; }
    public bool IsActive { get; private set; }
    public string? Description { get; private set; }

    // ISoftDelete implementation
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Features
    public bool HasProjector { get; private set; }
    public bool HasSmartBoard { get; private set; }
    public bool HasComputers { get; private set; }
    public bool HasAirConditioning { get; private set; }
    public int? ComputerCount { get; private set; }

    private Classroom() { } // EF Core

    private Classroom(string code, string name, int capacity, ClassroomType type)
    {
        Code = code;
        Name = name;
        Capacity = capacity;
        Type = type;
        IsActive = true;
        IsDeleted = false;
    }

    public static Classroom Create(string code, string name, int capacity, ClassroomType type)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Derslik kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Derslik adı boş olamaz.");

        if (capacity <= 0)
            throw new DomainException("Kapasite 0'dan büyük olmalıdır.");

        return new Classroom(code, name, capacity, type);
    }

    public void UpdateBasicInfo(string name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Derslik adı boş olamaz.");

        if (capacity <= 0)
            throw new DomainException("Kapasite 0'dan büyük olmalıdır.");

        Name = name;
        Capacity = capacity;
    }

    public void SetLocation(string building, int floor)
    {
        if (string.IsNullOrWhiteSpace(building))
            throw new DomainException("Bina adı boş olamaz.");

        if (floor < 0)
            throw new DomainException("Kat numarası negatif olamaz.");

        Building = building;
        Floor = floor;
    }

    public void SetFeatures(bool hasProjector, bool hasSmartBoard, bool hasComputers,
        bool hasAirConditioning, int? computerCount = null)
    {
        HasProjector = hasProjector;
        HasSmartBoard = hasSmartBoard;
        HasComputers = hasComputers;
        HasAirConditioning = hasAirConditioning;

        if (hasComputers && (!computerCount.HasValue || computerCount.Value <= 0))
            throw new DomainException("Bilgisayarlı derslik için bilgisayar sayısı belirtilmelidir.");

        ComputerCount = hasComputers ? computerCount : null;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }

    public bool CanAccommodate(int requiredCapacity)
    {
        return IsActive && !IsDeleted && Capacity >= requiredCapacity;
    }

    public bool HasRequiredFeatures(bool needsProjector, bool needsComputers)
    {
        if (needsProjector && !HasProjector)
            return false;

        if (needsComputers && !HasComputers)
            return false;

        return true;
    }
}