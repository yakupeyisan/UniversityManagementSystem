using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Attendances.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Attendances.Queries;


// ===== GetStudentAttendanceQuery =====
public record GetStudentAttendanceQuery(
    Guid StudentId,
    Guid CourseId
) : IRequest<Result<StudentAttendanceDto>>;

public class GetStudentAttendanceQueryHandler : IRequestHandler<GetStudentAttendanceQuery, Result<StudentAttendanceDto>>
{
    private readonly IRepository<Attendance> _attendanceRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStudentAttendanceQueryHandler> _logger;

    public GetStudentAttendanceQueryHandler(
        IRepository<Attendance> attendanceRepository,
        IMapper mapper,
        ILogger<GetStudentAttendanceQueryHandler> logger)
    {
        _attendanceRepository = attendanceRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<StudentAttendanceDto>> Handle(
        GetStudentAttendanceQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var attendances = await _attendanceRepository.FindAsync(
                a => a.StudentId == request.StudentId && a.CourseId == request.CourseId,
                cancellationToken);

            if (!attendances.Any())
            {
                return Result.Success(new StudentAttendanceDto
                {
                    StudentId = request.StudentId,
                    CourseId = request.CourseId,
                    TotalSessions = 0,
                    PresentCount = 0,
                    AbsentCount = 0,
                    AttendanceRate = 100.0,
                    Attendances = new List<AttendanceDto>()
                });
            }

            var attendanceDtos = _mapper.Map<List<AttendanceDto>>(attendances);

            var studentAttendance = new StudentAttendanceDto
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                TotalSessions = attendances.Count,
                PresentCount = attendances.Count(a => a.IsPresent),
                AbsentCount = attendances.Count(a => !a.IsPresent),
                AttendanceRate = (double)attendances.Count(a => a.IsPresent) / attendances.Count * 100,
                Attendances = attendanceDtos.OrderBy(a => a.AttendanceDate).ToList()
            };

            return Result.Success(studentAttendance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student attendance. StudentId: {StudentId}, CourseId: {CourseId}",
                request.StudentId, request.CourseId);
            return Result.Failure<StudentAttendanceDto>("Devamsızlık bilgileri alınırken bir hata oluştu.");
        }
    }
}

// ===== GetAttendanceReportQuery =====
public record GetAttendanceReportQuery(
    Guid CourseId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<Result<AttendanceReportDto>>;

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
                return Result.Failure<AttendanceReportDto>("Bu ders için yoklama kaydı bulunamadı.");

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

            return Result.Success(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating attendance report. CourseId: {CourseId}", request.CourseId);
            return Result.Failure<AttendanceReportDto>("Devamsızlık raporu oluşturulurken bir hata oluştu.");
        }
    }
}
