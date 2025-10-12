using UniversityMS.Domain.Entities.Common;

namespace UniversityMS.Domain.Entities.AcademicAggregate;

public class Prerequisite : AuditableEntity
{
    public Guid CourseId { get; private set; }
    public Guid PrerequisiteCourseId { get; private set; }

    // Navigation Properties
    public Course Course { get; private set; } = null!;
    public Course PrerequisiteCourse { get; private set; } = null!;

    private Prerequisite() { } // EF Core

    private Prerequisite(Guid courseId, Guid prerequisiteCourseId)
        : base()
    {
        CourseId = courseId;
        PrerequisiteCourseId = prerequisiteCourseId;
    }

    public static Prerequisite Create(Guid courseId, Guid prerequisiteCourseId)
    {
        return new Prerequisite(courseId, prerequisiteCourseId);
    }
}