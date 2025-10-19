using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.FacultyFeature.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.FacultyFeature.Queries;

public class GetFacultyByIdQueryHandler : IRequestHandler<GetFacultyByIdQuery, Result<FacultyDto>>
{
    private readonly IRepository<Faculty> _facultyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetFacultyByIdQueryHandler> _logger;

    public GetFacultyByIdQueryHandler(
        IRepository<Faculty> facultyRepository,
        IMapper mapper,
        ILogger<GetFacultyByIdQueryHandler> logger)
    {
        _facultyRepository = facultyRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<FacultyDto>> Handle(GetFacultyByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var faculty = await _facultyRepository.GetByIdAsync(request.Id, cancellationToken);

            if (faculty == null)
            {
                _logger.LogWarning("Faculty not found. FacultyId: {FacultyId}", request.Id);
                return Result<FacultyDto>.Failure("Fakülte bulunamadı.");
            }

            var facultyDto = _mapper.Map<FacultyDto>(faculty);
            return Result<FacultyDto>.Success(facultyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving faculty. FacultyId: {FacultyId}", request.Id);
            return Result<FacultyDto>.Failure("Fakülte bilgileri alınırken bir hata oluştu.");
        }
    }
}