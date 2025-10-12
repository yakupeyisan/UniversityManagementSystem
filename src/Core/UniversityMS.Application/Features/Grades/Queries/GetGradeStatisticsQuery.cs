using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Grades.DTOs;

namespace UniversityMS.Application.Features.Grades.Queries;

public record GetGradeStatisticsQuery(Guid CourseId) : IRequest<Result<GradeStatisticsDto>>;