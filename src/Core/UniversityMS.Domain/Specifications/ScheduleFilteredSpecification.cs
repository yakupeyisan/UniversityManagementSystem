using System.Linq.Expressions;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Specifications;

public class ScheduleFilteredSpecification : ISpecification<Schedule>
{
    public Expression<Func<Schedule, bool>>? Criteria { get; }
    public List<Expression<Func<Schedule, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<Schedule, object>>? OrderBy { get; }
    public Expression<Func<Schedule, object>>? OrderByDescending { get; }
    public List<Expression<Func<Schedule, object>>> OrderByDescriptors { get; } = new();
    public List<bool> IsOrderByDescending { get; } = new();
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    public ScheduleFilteredSpecification(
        string? academicYear = null,
        int? semester = null,
        Guid? departmentId = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        // Filtreleme kriteri
        Expression<Func<Schedule, bool>> criteria = s =>
            !s.IsDeleted &&
            (string.IsNullOrEmpty(academicYear) || s.AcademicYear == academicYear) &&
            (!semester.HasValue || s.Semester == semester.Value) &&
            (!departmentId.HasValue || s.DepartmentId == departmentId.Value);

        Criteria = criteria;

        // Navigation properties include et
        Includes.Add(s => s.CourseSessions);

        // Sıralama
        OrderByDescending = s => s.CreatedAt;

        // Pagination
        Skip = (pageNumber - 1) * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }
}