using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

public class GetPayrollByEmployeeQuery : IRequest<Result<List<PayrollDto>>>
{

    public Guid EmployeeId { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
    public GetPayrollByEmployeeQuery(Guid employeeId, int? month, int? year)
    {
        EmployeeId = employeeId;
        Month = month;
        Year = year;
    }
}