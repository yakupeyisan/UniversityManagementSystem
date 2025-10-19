using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.AttendanceFeature.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.AttendanceFeature.Queries;

public class GetAttendanceReportQueryHandler
    : IRequestHandler<GetAttendanceReportQuery, Result<AttendanceReportDto>>
{
    private readonly IRepository<Attendance> _attendanceRepository;
    private readonly IRepository<CourseRegistration> _courseRegistrationRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAttendanceReportQueryHandler> _logger;

    public GetAttendanceReportQueryHandler(
        IRepository<Attendance> attendanceRepository,
        IRepository<CourseRegistration> courseRegistrationRepository,
        IMapper mapper,
        ILogger<GetAttendanceReportQueryHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _courseRegistrationRepository = courseRegistrationRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<AttendanceReportDto>> Handle(
        GetAttendanceReportQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var courseRegistrations = await _courseRegistrationRepository.FindAsync(
                cr => cr.CourseId == request.CourseId,
                cancellationToken);

            if (!courseRegistrations.Any())
                return Result<AttendanceReportDto>.Failure("Ders kaydı bulunamadı.");

            var attendances = new List<Attendance>();
            foreach (var cr in courseRegistrations)
            {
                var records = await _attendanceRepository.FindAsync(
                    a => a.CourseRegistrationId == cr.Id,
                    cancellationToken);
                attendances.AddRange(records);
            }

            if (request.StartDate.HasValue)
                attendances = attendances.Where(a => a.CreatedAt >= request.StartDate).ToList();

            if (request.EndDate.HasValue)
                attendances = attendances.Where(a => a.CreatedAt <= request.EndDate).ToList();

            var report = new AttendanceReportDto
            {
                CourseId = request.CourseId,
                TotalStudents = courseRegistrations.Count,
                TotalSessions = attendances.GroupBy(a => a.WeekNumber).Count(),
                OverallPresentPercentage = CalculatePresentPercentage(attendances),
                GeneratedDate = DateTime.UtcNow,
                StudentAttendances = GenerateStudentReport(attendances, courseRegistrations)
            };

            return Result<AttendanceReportDto>.Success(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating attendance report");
            return Result<AttendanceReportDto>.Failure("Rapor oluşturulurken bir hata oluştu.", ex.Message);
        }
    }

    private decimal CalculatePresentPercentage(List<Attendance> attendances)
    {
        if (!attendances.Any()) return 0;
        var presentCount = attendances.Count(a => a.IsPresent);
        return (presentCount * 100m) / attendances.Count;
    }

    private List<StudentAttendanceReportItemDto> GenerateStudentReport(
        List<Attendance> attendances,
        IEnumerable<CourseRegistration> courseRegistrations)
    {
        return courseRegistrations.Select(cr =>
        {
            var crAttendances = attendances.Where(a => a.CourseRegistrationId == cr.Id).ToList();
            var present = crAttendances.Count(a => a.IsPresent);
            var total = crAttendances.Count;

            return new StudentAttendanceReportItemDto
            {
                StudentId = cr.Enrollment.StudentId,
                PresentCount = present,
                AbsentCount = total - present,
                AttendancePercentage = total > 0 ? (present * 100m) / total : 0
            };
        }).ToList();
    }
}
