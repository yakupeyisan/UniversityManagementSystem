using System.Linq.Expressions;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Specifications;


public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
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
}

public class StudentByNumberSpec : BaseSpecification<Student>
{
    public StudentByNumberSpec(string studentNumber)
        : base(s => s.StudentNumber == studentNumber)
    {
        AddInclude(s => s.Address!);
        AddInclude(s => s.EmergencyContact!);
    }
}

public class StudentsByDepartmentSpec : BaseSpecification<Student>
{
    public StudentsByDepartmentSpec(Guid departmentId, StudentStatus? status = null)
        : base(s => s.DepartmentId == departmentId &&
                    (!status.HasValue || s.Status == status.Value))
    {
        ApplyOrderBy(s => s.StudentNumber);
    }
}

public class ActiveStudentsSpec : BaseSpecification<Student>
{
    public ActiveStudentsSpec()
        : base(s => s.Status == StudentStatus.Active && !s.IsDeleted)
    {
        ApplyOrderBy(s => s.StudentNumber);
    }
}

public class StudentsByEducationLevelSpec : BaseSpecification<Student>
{
    public StudentsByEducationLevelSpec(EducationLevel educationLevel)
        : base(s => s.EducationLevel == educationLevel && s.Status == StudentStatus.Active)
    {
        ApplyOrderBy(s => s.CurrentSemester);
    }
}

public class StudentsWithLowGPASpec : BaseSpecification<Student>
{
    public StudentsWithLowGPASpec(double cgpaThreshold = 2.0)
        : base(s => s.CGPA < cgpaThreshold && s.Status == StudentStatus.Active)
    {
        ApplyOrderBy(s => s.CGPA);
    }
}

public class StaffByEmployeeNumberSpec : BaseSpecification<Staff>
{
    public StaffByEmployeeNumberSpec(string employeeNumber)
        : base(s => s.EmployeeNumber == employeeNumber)
    {
        AddInclude(s => s.Address!);
        AddInclude(s => s.EmergencyContact!);
    }
}

public class ActiveStaffSpec : BaseSpecification<Staff>
{
    public ActiveStaffSpec()
        : base(s => s.IsActive && !s.IsDeleted)
    {
        ApplyOrderBy(s => s.EmployeeNumber);
    }
}

public class StaffByDepartmentSpec : BaseSpecification<Staff>
{
    public StaffByDepartmentSpec(Guid departmentId)
        : base(s => s.DepartmentId == departmentId && s.IsActive)
    {
        ApplyOrderBy(s => s.EmployeeNumber);
    }
}

public class AcademicStaffSpec : BaseSpecification<Staff>
{
    public AcademicStaffSpec(Guid? departmentId = null)
        : base(s => s.AcademicTitle.HasValue &&
                    s.IsActive &&
                    (!departmentId.HasValue || s.DepartmentId == departmentId))
    {
        ApplyOrderBy(s => s.AcademicTitle);
    }
}

public class StaffByAcademicTitleSpec : BaseSpecification<Staff>
{
    public StaffByAcademicTitleSpec(AcademicTitle academicTitle)
        : base(s => s.AcademicTitle == academicTitle && s.IsActive)
    {
        ApplyOrderBy(s => s.HireDate);
    }
}

public class UserByUsernameSpec : BaseSpecification<User>
{
    public UserByUsernameSpec(string username)
        : base(u => u.Username == username && !u.IsDeleted)
    {
        AddInclude(u => u.UserRoles);
        AddInclude("UserRoles.Role");
        AddInclude("UserRoles.Role.Permissions");
    }
}

public class UserByEmailSpec : BaseSpecification<User>
{
    public UserByEmailSpec(string email)
        : base(u => u.Email.Value == email && !u.IsDeleted)
    {
        AddInclude(u => u.UserRoles);
    }
}

public class ActiveUsersSpec : BaseSpecification<User>
{
    public ActiveUsersSpec()
        : base(u => u.IsActive && !u.IsDeleted)
    {
        ApplyOrderBy(u => u.Username);
    }
}