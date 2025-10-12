using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Grades.DTOs;

namespace UniversityMS.Application.Features.Grades.Queries;

public record GetTranscriptQuery(Guid StudentId) : IRequest<Result<TranscriptDto>>;