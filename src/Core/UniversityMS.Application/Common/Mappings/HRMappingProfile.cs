using AutoMapper;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;

namespace UniversityMS.Application.Common.Mappings;

/// <summary>
/// HR Modülü AutoMapper Profile'ı
/// Entity'leri DTO'lara ve tersine map eder
/// </summary>
public class HRMappingProfile : Profile
{
    public HRMappingProfile()
    {
        // ============================================================
        // EMPLOYEE MAPPINGS
        // ============================================================

        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.EmployeeNumber,
                opt => opt.MapFrom(src => src.EmployeeNumber.Value))
            .ForMember(dest => dest.PersonName,
                opt => opt.MapFrom(src => $"{src.Person.FirstName} {src.Person.LastName}"))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Person.Email.Value))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.BaseSalary,
                opt => opt.MapFrom(src => src.Salary.GetGrossSalary()))
            .ForMember(dest => dest.Department,
                opt => opt.MapFrom(src => src.Department.Name))
            .ReverseMap();

        CreateMap<Employee, EmployeeListDto>()
            .ForMember(dest => dest.EmployeeNumber,
                opt => opt.MapFrom(src => src.EmployeeNumber.Value))
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.Person.FirstName} {src.Person.LastName}"))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Person.Email.Value))
            .ForMember(dest => dest.Department,
                opt => opt.MapFrom(src => src.Department.Name))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));

        // ============================================================
        // LEAVE MAPPINGS
        // ============================================================

        CreateMap<Leave, LeaveRequestDto>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.LeaveType,
                opt => opt.MapFrom(src => src.LeaveType.ToString()))
            .ForMember(dest => dest.DurationDays,
                opt => opt.MapFrom(src => (int)(src.EndDate - src.StartDate).TotalDays + 1));

        CreateMap<Leave, LeaveDetailDto>()
            .ForMember(dest => dest.EmployeeNumber,
                opt => opt.MapFrom(src => src.Employee.EmployeeNumber.Value))
            .ForMember(dest => dest.EmployeeName,
                opt => opt.MapFrom(src => $"{src.Employee.Person.FirstName} {src.Employee.Person.LastName}"))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.LeaveType,
                opt => opt.MapFrom(src => src.LeaveType.ToString()))
            .ForMember(dest => dest.DurationDays,
                opt => opt.MapFrom(src => (int)(src.EndDate - src.StartDate).TotalDays + 1));

        // ============================================================
        // SHIFT MAPPINGS
        // ============================================================

        CreateMap<Shift, ShiftDto>()
            .ForMember(dest => dest.EmployeeNumber,
                opt => opt.MapFrom(src => src.Employee.EmployeeNumber.Value))
            .ForMember(dest => dest.EmployeeName,
                opt => opt.MapFrom(src => $"{src.Employee.Person.FirstName} {src.Employee.Person.LastName}"))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ShiftPattern,
                opt => opt.MapFrom(src => src.ShiftPattern.ToString()));

        // ============================================================
        // CONTRACT MAPPINGS
        // ============================================================

        CreateMap<Contract, ContractDto>()
            .ForMember(dest => dest.EmployeeNumber,
                opt => opt.MapFrom(src => src.Employee.EmployeeNumber.Value))
            .ForMember(dest => dest.EmployeeName,
                opt => opt.MapFrom(src => $"{src.Employee.Person.FirstName} {src.Employee.Person.LastName}"))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ContractType,
                opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Salary,
                opt => opt.MapFrom(src => src.Salary.GetGrossSalary()));

        CreateMap<Contract, ContractDetailDto>()
            .ForMember(dest => dest.EmployeeNumber,
                opt => opt.MapFrom(src => src.Employee.EmployeeNumber.Value))
            .ForMember(dest => dest.EmployeeName,
                opt => opt.MapFrom(src => $"{src.Employee.Person.FirstName} {src.Employee.Person.LastName}"))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ContractType,
                opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Salary,
                opt => opt.MapFrom(src => src.Salary.GetGrossSalary()));

        // ============================================================
        // PERFORMANCE REVIEW MAPPINGS
        // ============================================================

        CreateMap<PerformanceReview, PerformanceReviewDto>()
            .ForMember(dest => dest.EmployeeName,
                opt => opt.MapFrom(src => $"{src.Employee.Person.FirstName} {src.Employee.Person.LastName}"))
            .ForMember(dest => dest.ReviewerName,
                opt => opt.MapFrom(src => $"{src.Reviewer.Person.FirstName} {src.Reviewer.Person.LastName}"))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.OverallRating,
                opt => opt.MapFrom(src => src.OverallRating.ToString()))
            .ForMember(dest => dest.OverallScore,
                opt => opt.MapFrom(src => src.OverallScore.Score));

        CreateMap<PerformanceReview, PerformanceReviewDetailDto>()
            .ForMember(dest => dest.EmployeeName,
                opt => opt.MapFrom(src => $"{src.Employee.Person.FirstName} {src.Employee.Person.LastName}"))
            .ForMember(dest => dest.EmployeeNumber,
                opt => opt.MapFrom(src => src.Employee.EmployeeNumber.Value))
            .ForMember(dest => dest.ReviewerName,
                opt => opt.MapFrom(src => $"{src.Reviewer.Person.FirstName} {src.Reviewer.Person.LastName}"))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.OverallRating,
                opt => opt.MapFrom(src => src.OverallRating.ToString()))
            .ForMember(dest => dest.OverallScore,
                opt => opt.MapFrom(src => src.OverallScore.Score))
            .ForMember(dest => dest.ApprovedByName,
                opt => opt.MapFrom(src => src.Approver != null
                    ? $"{src.Approver.Person.FirstName} {src.Approver.Person.LastName}"
                    : null));

        // ============================================================
        // TRAINING MAPPINGS
        // ============================================================

        CreateMap<Training, TrainingDto>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Duration,
                opt => opt.MapFrom(src => src.Duration.Hours))
            .ForMember(dest => dest.ParticipantCount,
                opt => opt.MapFrom(src => src.Participants.Count));

        CreateMap<Training, TrainingDetailDto>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Duration,
                opt => opt.MapFrom(src => src.Duration.Hours))
            .ForMember(dest => dest.ParticipantCount,
                opt => opt.MapFrom(src => src.Participants.Count))
            .ForMember(dest => dest.Participants,
                opt => opt.MapFrom(src => src.Participants));

        CreateMap<TrainingParticipant, TrainingParticipantDto>()
            .ForMember(dest => dest.EmployeeName,
                opt => opt.MapFrom(src => $"{src.Employee.Person.FirstName} {src.Employee.Person.LastName}"))
            .ForMember(dest => dest.EmployeeNumber,
                opt => opt.MapFrom(src => src.Employee.EmployeeNumber.Value))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));

        // ============================================================
        // CREATE DTOs MAPPINGS
        // ============================================================

        CreateMap<CreateEmployeeDto, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}