using UniversityMS.Domain.Entities.PayrollAggregate;

namespace UniversityMS.Domain.Specifications;

public class PayrollByEmployeeSpecification : BaseSpecification<Payroll>
{
    public PayrollByEmployeeSpecification(
        Guid employeeId,
        int? month,
        int? year)
        : base(p =>
            p.EmployeeId == employeeId &&
            (!month.HasValue || p.Month == month) &&
            (!year.HasValue || p.Year == year))
    {
        AddInclude(p => p.Employee);
        AddOrderByDescending(p => p.Year);
        AddOrderByDescending(p => p.Month);
    }
}