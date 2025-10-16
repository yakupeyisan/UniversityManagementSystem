using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Attendances.DTOs;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Attendances.Queries;

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
                return Result<StudentAttendanceDto>.Success(new StudentAttendanceDto
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

            return Result<StudentAttendanceDto>.Success(studentAttendance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student attendance. StudentId: {StudentId}, CourseId: {CourseId}",
                request.StudentId, request.CourseId);
            return Result<StudentAttendanceDto>.Failure("Devamsızlık bilgileri alınırken bir hata oluştu.");
        }
    }
}