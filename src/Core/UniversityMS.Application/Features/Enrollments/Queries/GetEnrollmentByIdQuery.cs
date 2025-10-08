using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Enrollments.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Enrollments.Queries;


public record GetEnrollmentByIdQuery(Guid Id) : IRequest<Result<EnrollmentDto>>;

public class GetEnrollmentByIdQueryHandler : IRequestHandler<GetEnrollmentByIdQuery, Result<EnrollmentDto>>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetEnrollmentByIdQueryHandler> _logger;

    public GetEnrollmentByIdQueryHandler(
        IRepository<Enrollment> enrollmentRepository,
        IMapper mapper,
        ILogger<GetEnrollmentByIdQueryHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<EnrollmentDto>> Handle(GetEnrollmentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.Id, cancellationToken);

            if (enrollment == null)
            {
                _logger.LogWarning("Enrollment not found. EnrollmentId: {EnrollmentId}", request.Id);
                return Result.Failure<EnrollmentDto>("Kayıt bulunamadı.");
            }

            var enrollmentDto = _mapper.Map<EnrollmentDto>(enrollment);
            return Result.Success(enrollmentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enrollment. EnrollmentId: {EnrollmentId}", request.Id);
            return Result.Failure<EnrollmentDto>("Kayıt bilgileri alınırken bir hata oluştu.");
        }
    }
}
public record GetStudentEnrollmentsQuery(
    Guid StudentId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedList<EnrollmentDto>>>;

public class GetStudentEnrollmentsQueryHandler : IRequestHandler<GetStudentEnrollmentsQuery, Result<PaginatedList<EnrollmentDto>>>
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStudentEnrollmentsQueryHandler> _logger;

    public GetStudentEnrollmentsQueryHandler(
        IRepository<Enrollment> enrollmentRepository,
        IMapper mapper,
        ILogger<GetStudentEnrollmentsQueryHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<EnrollmentDto>>> Handle(
        GetStudentEnrollmentsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Enrollment, bool>> predicate = e => e.StudentId == request.StudentId;

            var (enrollments, totalCount) = await _enrollmentRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                cancellationToken);

            var enrollmentDtos = _mapper.Map<List<EnrollmentDto>>(enrollments);
            var paginatedList = new PaginatedList<EnrollmentDto>(
                enrollmentDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result.Success(paginatedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student enrollments. StudentId: {StudentId}", request.StudentId);
            return Result.Failure<PaginatedList<EnrollmentDto>>("Öğrenci kayıtları alınırken bir hata oluştu.");
        }
    }
}