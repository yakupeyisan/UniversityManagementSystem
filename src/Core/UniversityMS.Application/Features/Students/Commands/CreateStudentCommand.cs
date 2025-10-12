using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Students.Commands;

public record CreateStudentCommand(
    string FirstName,
    string LastName,
    string NationalId,
    DateTime BirthDate,
    Gender Gender,
    string Email,
    string PhoneNumber,
    string StudentNumber,
    Guid DepartmentId,
    EducationLevel EducationLevel
) : IRequest<Result<Guid>>;