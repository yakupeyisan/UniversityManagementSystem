using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.StaffFeature.Commands;


public class CreateEmployeeCommand : IRequest<Result<Guid>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Position { get; set; }
    public Guid DepartmentId { get; set; }
    public string EmploymentType { get; set; }
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