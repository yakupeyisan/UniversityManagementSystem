using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Queries;

/// <summary>
/// Çalışanı ID'ye göre getir Handler
/// </summary>
public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
{
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetEmployeeByIdQueryHandler> _logger;

    public GetEmployeeByIdQueryHandler(
        IRepository<Employee> employeeRepository,
        IMapper mapper,
        ILogger<GetEmployeeByIdQueryHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<EmployeeDto>> Handle(
        GetEmployeeByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Çalışan getiriliyor. EmployeeId: {EmployeeId}", request.EmployeeId);

            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);

            if (employee == null)
            {
                _logger.LogWarning("Çalışan bulunamadı. EmployeeId: {EmployeeId}", request.EmployeeId);
                return Result<EmployeeDto>.Failure("Çalışan bulunamadı.");
            }

            var dto = _mapper.Map<EmployeeDto>(employee);

            _logger.LogInformation(
                "Çalışan başarıyla getirилді. EmployeeId: {EmployeeId}, Name: {Name}",
                request.EmployeeId, $"{employee.Person.FirstName} {employee.Person.LastName}");

            return Result<EmployeeDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çalışan getirme hatası. EmployeeId: {EmployeeId}", request.EmployeeId);
            return Result<EmployeeDto>.Failure($"Çalışan getirme hatası: {ex.Message}");
        }
    }
}
