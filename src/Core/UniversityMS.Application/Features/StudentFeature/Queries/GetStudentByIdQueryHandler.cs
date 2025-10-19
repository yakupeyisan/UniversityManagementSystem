using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StudentFeature.DTOs;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.StudentFeature.Queries;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentDto>>
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStudentByIdQueryHandler> _logger;

    public GetStudentByIdQueryHandler(
        IRepository<Student> studentRepository,
        IMapper mapper,
        ILogger<GetStudentByIdQueryHandler> logger)
    {
        _studentRepository = studentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);

            if (student == null)
            {
                _logger.LogWarning("Student not found. StudentId: {StudentId}", request.Id);
                return Result<StudentDto>.Failure("Öğrenci bulunamadı.");
            }

            var studentDto = _mapper.Map<StudentDto>(student);
            return Result<StudentDto>.Success(studentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving student. StudentId: {StudentId}", request.Id);
            return Result<StudentDto>.Failure("Öğrenci bilgileri alınırken bir hata oluştu.");
        }
    }
}