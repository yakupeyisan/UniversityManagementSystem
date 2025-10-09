using AutoMapper;
using System.Data;
using UniversityMS.Application.Features.Attendances.DTOs;
using UniversityMS.Application.Features.Authentication.DTOs;
using UniversityMS.Application.Features.Courses.DTOs;
using UniversityMS.Application.Features.Departments.DTOs;
using UniversityMS.Application.Features.Enrollments.DTOs;
using UniversityMS.Application.Features.Faculties.DTOs;
using UniversityMS.Application.Features.Grades.DTOs;
using UniversityMS.Application.Features.Staff.DTOs;
using UniversityMS.Application.Features.Students.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Application.Common.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Student Mappings
        CreateMap<Student, StudentDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.GetFullName()))
            .ForMember(d => d.Age, opt => opt.MapFrom(s => s.GetAge()))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber.Value));

        // Staff Mappings
        CreateMap<Staff, StaffDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.GetFullName()))
            .ForMember(d => d.Age, opt => opt.MapFrom(s => s.GetAge()))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber.Value))
            .ForMember(d => d.YearsOfService, opt => opt.MapFrom(s => s.GetYearsOfService()));

        // User Mappings
        CreateMap<User, UserDto>()
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value))
            .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.UserRoles.Select(ur => ur.Role.Name)));

        // Role Mappings
        CreateMap<Role, RoleDto>();

        // Address Mappings
        CreateMap<Address, AddressDto>();

        // Emergency Contact Mappings
        CreateMap<EmergencyContact, EmergencyContactDto>()
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber.Value))
            .ForMember(d => d.AlternativePhone, opt => opt.MapFrom(s =>
                s.AlternativePhone != null ? s.AlternativePhone.Value : null));


        // Faculty Mappings
        CreateMap<Faculty, FacultyDto>()
            .ForMember(d => d.DepartmentCount, opt => opt.MapFrom(s => s.Departments.Count));

        // Department Mappings
        CreateMap<Department, DepartmentDto>()
            .ForMember(d => d.FacultyName, opt => opt.MapFrom(s => s.Faculty.Name))
            .ForMember(d => d.CourseCount, opt => opt.MapFrom(s => s.Courses.Count));

        // Course Mappings
        CreateMap<Course, CourseDto>()
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department.Name))
            .ForMember(d => d.TotalWeeklyHours, opt => opt.MapFrom(s => s.GetTotalWeeklyHours()))
            .ForMember(d => d.Prerequisites, opt => opt.MapFrom(s => s.Prerequisites.Select(p => new PrerequisiteCourseDto
            {
                CourseId = p.PrerequisiteCourseId,
                CourseName = p.PrerequisiteCourse.Name,
                CourseCode = p.PrerequisiteCourse.Code
            })));

        // Enrollment Mappings
        CreateMap<Enrollment, EnrollmentDto>();

        CreateMap<CourseRegistration, CourseRegistrationDto>()
            .ForMember(d => d.CourseName, opt => opt.MapFrom(s => s.Course.Name))
            .ForMember(d => d.CourseCode, opt => opt.MapFrom(s => s.Course.Code));

        // Grade Mappings
        CreateMap<Grade, GradeDto>()
            .ForMember(d => d.CourseName, opt => opt.MapFrom(s => s.CourseRegistration.Course.Name))
            .ForMember(d => d.CourseCode, opt => opt.MapFrom(s => s.CourseRegistration.Course.Code))
            .ForMember(d => d.InstructorId, opt => opt.MapFrom(s => s.InstructorId)); ;

        // Attendance Mappings
        CreateMap<Attendance, AttendanceDto>();
    }
}
