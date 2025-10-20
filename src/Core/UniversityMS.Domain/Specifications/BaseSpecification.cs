using System.Linq.Expressions;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.FacilityAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Filters;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Specifications;


public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public List<Expression<Func<T, object>>> OrderByDescriptors { get; } = new();
    public List<bool> IsOrderByDescending { get; } = new();
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    protected BaseSpecification()
    {
    }

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescending = orderByDescExpression;
    }
    protected virtual void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected virtual void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }
    protected virtual void AddThenBy(Expression<Func<T, object>> thenByExpression)
    {
        OrderByDescriptors.Add(thenByExpression);
        IsOrderByDescending.Add(false);
    }

    protected virtual void AddThenByDescending(Expression<Func<T, object>> thenByDescendingExpression)
    {
        OrderByDescriptors.Add(thenByDescendingExpression);
        IsOrderByDescending.Add(true);
    }

}
public class ClassroomFilteredSpecification : FilteredSpecification<Classroom>
{
    public ClassroomFilteredSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        IFilterParser<Classroom> filterParser)
        : base(filterString, filterParser, pageNumber, pageSize)
    {
        AddOrderBy(c => c.Building);
        AddThenBy(c => c.Floor);
        AddThenBy(c => c.Code);
    }
}
public class StudentFilteredSpecification : FilteredSpecification<Student>
{
    public StudentFilteredSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        IFilterParser<Student> filterParser)
        : base(filterString, filterParser, pageNumber, pageSize)
    {
        AddInclude(s => s.Address!);
        AddInclude(s => s.EmergencyContact!);
        AddOrderBy(s => s.StudentNumber);
    }
}
public class FacultyFilteredSpecification : FilteredSpecification<Faculty>
{
    public FacultyFilteredSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        IFilterParser<Faculty> filterParser)
        : base(filterString, filterParser, pageNumber, pageSize)
    {
        AddInclude(f => f.Departments);
        AddOrderBy(f => f.Name);
    }
}
public class DepartmentFilteredSpecification : FilteredSpecification<Department>
{
    public DepartmentFilteredSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        IFilterParser<Department> filterParser)
        : base(filterString, filterParser, pageNumber, pageSize)
    {
        AddInclude(d => d.Faculty);
        AddOrderBy(d => d.Name);
    }
}
public class EnrollmentFilteredSpecification : FilteredSpecification<Enrollment>
{
    public EnrollmentFilteredSpecification(
        string? filterString,
        int pageNumber,
        int pageSize,
        IFilterParser<Enrollment> filterParser)
        : base(filterString, filterParser, pageNumber, pageSize)
    {
        AddInclude(e => e.Student);
        AddInclude(e => e.CourseRegistrations);
        AddOrderByDescending(e => e.AcademicYear);
        AddOrderByDescending(e => e.Semester);
    }
}