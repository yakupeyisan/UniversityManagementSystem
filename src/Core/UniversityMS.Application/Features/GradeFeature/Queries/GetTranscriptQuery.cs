using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.GradeFeature.DTOs;

namespace UniversityMS.Application.Features.GradeFeature.Queries;

public record GetTranscriptQuery(Guid StudentId) : IRequest<Result<TranscriptDto>>;