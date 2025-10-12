using AutoMapper;
using MediatR;
using System.Linq.Expressions;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Enrollments.Commands;

public record CreateEnrollmentCommand(
    Guid StudentId,
    string AcademicYear,
    int Semester
) : IRequest<Result<Guid>>;