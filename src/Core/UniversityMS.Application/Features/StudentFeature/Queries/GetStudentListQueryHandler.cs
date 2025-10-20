using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StudentFeature.DTOs;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Filters;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.StudentFeature.Queries;
public class GetStudentListQueryHandler
    : IRequestHandler<GetStudentListQuery, Result<PaginatedList<StudentDto>>>
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IFilterParser<Student> _filterParser;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStudentListQueryHandler> _logger;

    public GetStudentListQueryHandler(
        IRepository<Student> studentRepository,
        IFilterParser<Student> filterParser,
        IMapper mapper,
        ILogger<GetStudentListQueryHandler> logger)
    {
        _studentRepository = studentRepository;
        _filterParser = filterParser;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<StudentDto>>> Handle(
        GetStudentListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Student list requested. PageNumber: {PageNumber}, PageSize: {PageSize}, Filter: {Filter}",
                request.PageNumber,
                request.PageSize,
                request.Filter ?? "None");

            // ✅ SPECIFICATION PATTERN
            var specification = new StudentFilteredSpecification(
                filterString: request.Filter,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                filterParser: _filterParser);

            // ✅ Repository'den veri al (DATABASE'DE FILTRELEME)
            var students = await _studentRepository.ListAsync(specification, cancellationToken);

            // ✅ Total count (specification ile soft delete kontrol edilmiş)
            var totalCount = await _studentRepository.CountAsync(specification, cancellationToken);

            _logger.LogInformation(
                "Students retrieved. TotalCount: {TotalCount}, Returned: {ReturnedCount}",
                totalCount,
                students.Count);

            // DTO Mapping
            var studentDtos = _mapper.Map<List<StudentDto>>(students);

            var paginatedList = new PaginatedList<StudentDto>(
                studentDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result<PaginatedList<StudentDto>>.Success(paginatedList);
        }
        catch (FilterParsingException ex)
        {
            _logger.LogWarning(ex, "Invalid filter format: {Filter}", request.Filter);
            return Result<PaginatedList<StudentDto>>.Failure(
                $"Geçersiz filter formatı: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student list");
            return Result<PaginatedList<StudentDto>>.Failure(
                "Öğrenci listesi alınırken bir hata oluştu.");
        }
    }
}