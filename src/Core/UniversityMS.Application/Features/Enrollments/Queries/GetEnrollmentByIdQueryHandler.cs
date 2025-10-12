using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Enrollments.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Enrollments.Queries;

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