using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.AcademicAggregate;

public class CurriculumCourse : AuditableEntity // AuditableEntity'den inherit ediliyor artık
{
    public Guid CurriculumId { get; private set; }
    public Guid CourseId { get; private set; }
    public int Semester { get; private set; }
    public CourseType CourseType { get; private set; } // YENİ: Compulsory/Elective
    public bool IsElective { get; private set; } // YENİ: Seçmeli mi?

    // Navigation Properties
    public Curriculum Curriculum { get; private set; } = null!;
    public Course Course { get; private set; } = null!;

    private CurriculumCourse() { } // EF Core

    private CurriculumCourse(Guid curriculumId, Guid courseId, int semester,
        CourseType courseType, bool isElective)
        : base()
    {
        CurriculumId = curriculumId;
        CourseId = courseId;
        Semester = semester;
        CourseType = courseType;
        IsElective = isElective;
    }

    public static CurriculumCourse Create(Guid curriculumId, Guid courseId, int semester,
        CourseType courseType, bool isElective)
    {
        return new CurriculumCourse(curriculumId, courseId, semester, courseType, isElective);
    }

    public void UpdateSemester(int semester)
    {
        if (semester < 1 || semester > 8)
            throw new DomainException("Dönem 1-8 arasında olmalıdır.");

        Semester = semester;
    }

    public void SetElective(bool isElective)
    {
        IsElective = isElective;
        CourseType = isElective ? CourseType.Elective : CourseType.Compulsory;
    }
}