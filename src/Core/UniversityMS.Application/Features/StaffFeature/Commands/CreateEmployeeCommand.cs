using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.StaffFeature.Commands;


public class CreateEmployeeCommand : IRequest<Result<Guid>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public string EmploymentType { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }

    public CreateEmployeeCommand(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string position,
        Guid departmentId,
        string employmentType,
        DateTime hireDate
    )
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Position = position;
        DepartmentId = departmentId;
        EmploymentType = employmentType;
        HireDate = hireDate;
    }
}