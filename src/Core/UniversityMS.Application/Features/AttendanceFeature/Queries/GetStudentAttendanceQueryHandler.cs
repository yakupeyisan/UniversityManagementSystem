using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.AttendanceFeature.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.AttendanceFeature.Queries;

public class GetStudentAttendanceQueryHandler
    : IRequestHandler<GetStudentAttendanceQuery, Result<StudentAttendanceDto>>
{
    private readonly IRepository<Attendance> _attendanceRepository;
    private readonly IRepository<CourseRegistration> _courseRegistrationRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStudentAttendanceQueryHandler> _logger;

    public GetStudentAttendanceQueryHandler(
        IRepository<Attendance> attendanceRepository,
        IRepository<CourseRegistration> courseRegistrationRepository,
        IMapper mapper,
        ILogger<GetStudentAttendanceQueryHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _courseRegistrationRepository = courseRegistrationRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<StudentAttendanceDto>> Handle(
        GetStudentAttendanceQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var courseRegistrations = await _courseRegistrationRepository.FindAsync(
                cr => cr.Enrollment.StudentId == request.StudentId && cr.CourseId == request.CourseId,
                cancellationToken);

            if (!courseRegistrations.Any())
            {
                _logger.LogWarning("CourseRegistration not found for Student: {StudentId}, Course: {CourseId}",
                    request.StudentId, request.CourseId);
                return Result<StudentAttendanceDto>.Failure("Ders kaydı bulunamadı.");
            }

            var courseRegistration = courseRegistrations.First();
            var attendances = await _attendanceRepository.FindAsync(
                a => a.CourseRegistrationId == courseRegistration.Id,
                cancellationToken);

            var dto = new StudentAttendanceDto
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                TotalSessions = attendances.Count,
                PresentCount = attendances.Count(a => a.IsPresent),
                AbsentCount = attendances.Count(a => !a.IsPresent),
                AttendanceRecords = _mapper.Map<List<AttendanceRecordDto>>(attendances)
            };

            return Result<StudentAttendanceDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student attendance");
            return Result<StudentAttendanceDto>.Failure("Devam bilgileri alınırken bir hata oluştu.", ex.Message);
        }
    }
}
