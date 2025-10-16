using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Enrollments.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Enrollments.Queries;

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

            return Result<PaginatedList<EnrollmentDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student enrollments. StudentId: {StudentId}", request.StudentId);
            return Result<PaginatedList<EnrollmentDto>>.Failure("Öğrenci kayıtları alınırken bir hata oluştu.");
        }
    }
}