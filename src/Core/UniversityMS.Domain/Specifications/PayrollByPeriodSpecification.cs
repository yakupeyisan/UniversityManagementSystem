using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Entities.PayrollAggregate;

namespace UniversityMS.Domain.Specifications;

public class PayrollByPeriodSpecification : BaseSpecification<Payroll>
{
    public PayrollByPeriodSpecification(int month, int year)
        : base(p => p.Month == month && p.Year == year)
    {
        AddInclude(p => p.Employee);
        // Ordering belirtmeden, varsayılan sıralama kullan
    }
}