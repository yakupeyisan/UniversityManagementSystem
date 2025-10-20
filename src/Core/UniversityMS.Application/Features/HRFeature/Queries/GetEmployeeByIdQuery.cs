using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Queries;

/// Çalışanı ID'ye göre getir
/// </summary>
public record GetEmployeeByIdQuery(
    Guid EmployeeId
) : IRequest<Result<EmployeeDto>>;