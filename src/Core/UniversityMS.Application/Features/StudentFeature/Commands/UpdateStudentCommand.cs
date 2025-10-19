using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.StudentFeature.Commands;

public record UpdateStudentCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string PhoneNumber
) : IRequest<Result>;