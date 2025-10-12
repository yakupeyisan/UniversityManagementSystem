using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Commands;

public class ObjectToGradeCommandHandler : IRequestHandler<ObjectToGradeCommand, Result<Guid>>
{
    private readonly IRepository<GradeObjection> _objectionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ObjectToGradeCommandHandler> _logger;

    public ObjectToGradeCommandHandler(
        IRepository<GradeObjection> objectionRepository,
        IUnitOfWork unitOfWork,
        ILogger<ObjectToGradeCommandHandler> logger)
    {
        _objectionRepository = objectionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(ObjectToGradeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var objection = GradeObjection.Create(
                request.GradeId,
                request.StudentId,
                request.CourseId,
                request.Reason
            );

            await _objectionRepository.AddAsync(objection, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(objection.Id, "İtiraz kaydedildi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating objection");
            return Result.Failure<Guid>("İtiraz kaydedilemedi.");
        }
    }
}