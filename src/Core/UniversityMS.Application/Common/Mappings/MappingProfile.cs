using AutoMapper;
using System.Data;
using UniversityMS.Application.Features.Attendances.DTOs;
using UniversityMS.Application.Features.Authentication.DTOs;
using UniversityMS.Application.Features.Courses.DTOs;
using UniversityMS.Application.Features.Departments.DTOs;
using UniversityMS.Application.Features.Enrollments.DTOs;
using UniversityMS.Application.Features.Faculties.DTOs;
using UniversityMS.Application.Features.Grades.DTOs;
using UniversityMS.Application.Features.HR.DTOs;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Application.Features.Staff.DTOs;
using UniversityMS.Application.Features.Students.DTOs;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;

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
        // Payroll → PayrollDto
        CreateMap<Payroll, PayrollDto>()
            .ForMember(dest => dest.PayrollNumber, opt => opt.MapFrom(src => src.PayrollNumber))
            .ForMember(dest => dest.EmployeeFullName,
                opt => opt.MapFrom(src => $"{src.Employee.Person.FirstName} {src.Employee.Person.LastName}"))
            .ForMember(dest => dest.EmployeeNumber,
                opt => opt.MapFrom(src => src.Employee.EmployeeNumber))
            .ForMember(dest => dest.MonthName,
                opt => opt.MapFrom(src => GetMonthName(src.Month)))
            .ForMember(dest => dest.Period,
                opt => opt.MapFrom(src => $"{src.Month:D2}/{src.Year}"))
            .ForMember(dest => dest.BaseSalary,
                opt => opt.MapFrom(src => src.BaseSalary.Amount))
            .ForMember(dest => dest.TotalEarnings,
                opt => opt.MapFrom(src => src.TotalEarnings.Amount))
            .ForMember(dest => dest.TotalDeductions,
                opt => opt.MapFrom(src => src.TotalDeductions.Amount))
            .ForMember(dest => dest.NetSalary,
                opt => opt.MapFrom(src => src.NetSalary.Amount))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PaymentMethod,
                opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
            .ForMember(dest => dest.ApproverName,
                opt => opt.MapFrom(src => src.Approver != null
                    ? $"{src.Approver.Person.FirstName} {src.Approver.Person.LastName}"
                    : null));

        // Payroll → PayslipDto
        CreateMap<Payroll, PayslipDto>()
            .ForMember(dest => dest.PayrollNumber, opt => opt.MapFrom(src => src.PayrollNumber))
            .ForMember(dest => dest.EmployeeFullName,
                opt => opt.MapFrom(src => $"{src.Employee.Person.FirstName} {src.Employee.Person.LastName}"))
            .ForMember(dest => dest.EmployeeNumber,
                opt => opt.MapFrom(src => src.Employee.EmployeeNumber))
            .ForMember(dest => dest.Designation,
                opt => opt.MapFrom(src => src.Employee.JobTitle ?? "N/A"))
            .ForMember(dest => dest.Department,
                opt => opt.MapFrom(src => src.Employee.Department!.Name ?? "N/A"))
            .ForMember(dest => dest.Month,
                opt => opt.MapFrom(src => GetMonthName(src.Month)))
            .ForMember(dest => dest.BaseSalary,
                opt => opt.MapFrom(src => src.BaseSalary.Amount))
            .ForMember(dest => dest.TotalEarnings,
                opt => opt.MapFrom(src => src.TotalEarnings.Amount))
            .ForMember(dest => dest.TotalDeductions,
                opt => opt.MapFrom(src => src.TotalDeductions.Amount))
            .ForMember(dest => dest.NetSalary,
                opt => opt.MapFrom(src => src.NetSalary.Amount))
            .ForMember(dest => dest.PaymentMethod,
                opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
            .ForMember(dest => dest.Earnings,
                opt => opt.MapFrom(src => src.Items.Where(i => i.Type == PayrollItemType.Earning)
                    .Select(i => new PayslipLineItemDto
                    {
                        Description = i.Description,
                        Amount = i.Amount.Amount,
                        Quantity = i.Quantity
                    }).ToList()))
            .ForMember(dest => dest.Deductions,
                opt => opt.MapFrom(src => src.Deductions
                    .Select(d => new PayslipLineItemDto
                    {
                        Description = d.Description,
                        Amount = d.Amount.Amount
                    }).ToList()))
            .ForMember(dest => dest.GeneratedDate,
                opt => opt.MapFrom(src => DateTime.UtcNow));

        // PayrollItem → PayrollItemDto
        CreateMap<PayrollItem, PayrollItemDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount));

        // PayrollDeduction → PayrollDeductionDto
        CreateMap<PayrollDeduction, PayrollDeductionDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount));
    }
    private static string GetMonthName(int month) => month switch
    {
        1 => "Ocak",
        2 => "Şubat",
        3 => "Mart",
        4 => "Nisan",
        5 => "Mayıs",
        6 => "Haziran",
        7 => "Temmuz",
        8 => "Ağustos",
        9 => "Eylül",
        10 => "Ekim",
        11 => "Kasım",
        12 => "Aralık",
        _ => "Bilinmiyor"
    };
}
