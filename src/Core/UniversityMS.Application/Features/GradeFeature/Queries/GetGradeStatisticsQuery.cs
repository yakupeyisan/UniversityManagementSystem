using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.GradeFeature.DTOs;

namespace UniversityMS.Application.Features.GradeFeature.Queries;

public record GetGradeStatisticsQuery(Guid CourseId) : IRequest<Result<GradeStatisticsDto>>;