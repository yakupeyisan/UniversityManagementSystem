using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.StaffFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.StaffFeature.Queries;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
{
    private readonly IRepository<Staff> _staffRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetEmployeeByIdQueryHandler> _logger;

    public GetEmployeeByIdQueryHandler(
        IRepository<Staff> staffRepository,
        IMapper mapper,
        ILogger<GetEmployeeByIdQueryHandler> logger)
    {
        _staffRepository = staffRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<EmployeeDto>> Handle(
        GetEmployeeByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _staffRepository.GetByIdAsync(request.Id, cancellationToken);
            if (staff == null)
            {
                _logger.LogWarning("Employee not found. StaffId: {StaffId}", request.Id);
                return Result<EmployeeDto>.Failure("Çalışan bulunamadı.");
            }

            var dto = _mapper.Map<EmployeeDto>(staff);
            return Result<EmployeeDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employee");
            return Result<EmployeeDto>.Failure("Çalışan bilgileri alınırken bir hata oluştu. Hata: "+ ex.Message);
        }
    }
}