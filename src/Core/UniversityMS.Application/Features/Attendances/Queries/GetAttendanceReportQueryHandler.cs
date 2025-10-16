using System.Linq.Expressions;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Attendances.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Attendances.Queries;

public class GetAttendanceReportQueryHandler : IRequestHandler<GetAttendanceReportQuery, Result<AttendanceReportDto>>
{
    private readonly IRepository<Attendance> _attendanceRepository;
    private readonly ILogger<GetAttendanceReportQueryHandler> _logger;

    public GetAttendanceReportQueryHandler(
        IRepository<Attendance> attendanceRepository,
        ILogger<GetAttendanceReportQueryHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _logger = logger;
    }

    public async Task<Result<AttendanceReportDto>> Handle(
        GetAttendanceReportQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Attendance, bool>> predicate = a => a.CourseId == request.CourseId;

            if (request.StartDate.HasValue)
            {
                var startDate = request.StartDate.Value;
                predicate = predicate.And(a => a.AttendanceDate >= startDate);
            }

            if (request.EndDate.HasValue)
            {
                var endDate = request.EndDate.Value;
                predicate = predicate.And(a => a.AttendanceDate <= endDate);
            }

            var attendances = await _attendanceRepository.FindAsync(predicate, cancellationToken);

            if (!attendances.Any())
                return Result<AttendanceReportDto>.Failure("Bu ders için yoklama kaydı bulunamadı.");

            var report = new AttendanceReportDto
            {
                CourseId = request.CourseId,
                TotalSessions = attendances.Select(a => a.AttendanceDate.Date).Distinct().Count(),
                TotalStudentAttendances = attendances.Count,
                PresentCount = attendances.Count(a => a.IsPresent),
                AbsentCount = attendances.Count(a => !a.IsPresent),
                OverallAttendanceRate = (double)attendances.Count(a => a.IsPresent) / attendances.Count * 100,

                StudentAttendanceRates = attendances
                    .GroupBy(a => a.StudentId)
                    .Select(g => new StudentAttendanceRateDto
                    {
                        StudentId = g.Key,
                        TotalSessions = g.Count(),
                        PresentCount = g.Count(a => a.IsPresent),
                        AbsentCount = g.Count(a => !a.IsPresent),
                        AttendanceRate = (double)g.Count(a => a.IsPresent) / g.Count() * 100
                    })
                    .OrderBy(s => s.AttendanceRate)
                    .ToList(),

                WeeklyAttendance = attendances
                    .GroupBy(a => a.WeekNumber)
                    .Select(g => new WeeklyAttendanceDto
                    {
                        WeekNumber = g.Key,
                        TotalStudents = g.Count(),
                        PresentCount = g.Count(a => a.IsPresent),
                        AbsentCount = g.Count(a => !a.IsPresent),
                        AttendanceRate = (double)g.Count(a => a.IsPresent) / g.Count() * 100
                    })
                    .OrderBy(w => w.WeekNumber)
                    .ToList()
            };

            return Result<AttendanceReportDto>.Success(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating attendance report. CourseId: {CourseId}", request.CourseId);
            return Result<AttendanceReportDto>.Failure("Devamsızlık raporu oluşturulurken bir hata oluştu.");
        }
    }
}