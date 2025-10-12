using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UniversityMS.Application.Common.Extensions;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Students.DTOs;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Students.Queries;

public class GetStudentListQueryHandler
    : IRequestHandler<GetStudentListQuery, Result<PaginatedList<StudentDto>>>
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStudentListQueryHandler> _logger;

    public GetStudentListQueryHandler(
        IRepository<Student> studentRepository,
        IMapper mapper,
        ILogger<GetStudentListQueryHandler> logger)
    {
        _studentRepository = studentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<StudentDto>>> Handle(
        GetStudentListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Student, bool>> predicate = s => !s.IsDeleted;

            if (request.Status.HasValue)
            {
                var status = request.Status.Value;
                predicate = predicate.And(s => s.Status == status);
            }

            if (request.DepartmentId.HasValue)
            {
                var deptId = request.DepartmentId.Value;
                predicate = predicate.And(s => s.DepartmentId == deptId);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                predicate = predicate.And(s =>
                    s.FirstName.ToLower().Contains(searchTerm) ||
                    s.LastName.ToLower().Contains(searchTerm) ||
                    s.StudentNumber.ToLower().Contains(searchTerm));
            }

            var (students, totalCount) = await _studentRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate,
                cancellationToken);

            var studentDtos = _mapper.Map<List<StudentDto>>(students);
            var paginatedList = new PaginatedList<StudentDto>(
                studentDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result.Success(paginatedList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving student list");
            return Result.Failure<PaginatedList<StudentDto>>("Öğrenci listesi alınırken bir hata oluştu.");
        }
    }
}
