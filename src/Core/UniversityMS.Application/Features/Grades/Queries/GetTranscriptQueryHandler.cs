using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Grades.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Queries;

public class GetTranscriptQueryHandler : IRequestHandler<GetTranscriptQuery, Result<TranscriptDto>>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IRepository<Student> _studentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTranscriptQueryHandler> _logger;

    public GetTranscriptQueryHandler(
        IRepository<Grade> gradeRepository,
        IRepository<Student> studentRepository,
        IMapper mapper,
        ILogger<GetTranscriptQueryHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _studentRepository = studentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TranscriptDto>> Handle(GetTranscriptQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null)
                return Result.Failure<TranscriptDto>("Öğrenci bulunamadı.");

            var grades = await _gradeRepository.FindAsync(
                g => g.StudentId == request.StudentId,
                cancellationToken);

            var gradeDtos = _mapper.Map<List<GradeDto>>(grades);

            var transcript = new TranscriptDto
            {
                StudentId = student.Id,
                StudentNumber = student.StudentNumber,
                FullName = $"{student.FirstName} {student.LastName}",
                CGPA = student.CGPA,
                TotalCredits = student.TotalCredits,
                CompletedCredits = student.CompletedCredits,
                Grades = gradeDtos
            };

            return Result.Success(transcript);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating transcript");
            return Result.Failure<TranscriptDto>("Transkript oluşturulamadı.");
        }
    }
}