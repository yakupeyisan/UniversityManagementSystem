using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

public class GetMonthlyPayrollReportQuery : IRequest<Result<PayrollReportDto>>
{
    public int Month { get; set; }
    public int Year { get; set; }
}