using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public class CreateGradeObjectionCommandHandler : IRequestHandler<CreateGradeObjectionCommand, Result<Guid>>
{
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IRepository<GradeObjection> _objectionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateGradeObjectionCommandHandler> _logger;

    public CreateGradeObjectionCommandHandler(
        IRepository<Grade> gradeRepository,
        IRepository<GradeObjection> objectionRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateGradeObjectionCommandHandler> logger)
    {
        _gradeRepository = gradeRepository;
        _objectionRepository = objectionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateGradeObjectionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var grade = await _gradeRepository.GetByIdAsync(request.GradeId, cancellationToken);
            if (grade == null)
                return Result<Guid>.Failure("Not bulunamadı.");

            var objection = GradeObjection.Create(
                request.GradeId,
                request.StudentId,
                request.Reason,
                string.Empty  // Açıklama boş string
            );

            await _objectionRepository.AddAsync(objection, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Grade objection created. ObjectionId: {ObjectionId}", objection.Id);
            return Result<Guid>.Success(objection.Id, "İtiraz başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating grade objection");
            return Result<Guid>.Failure("İtiraz oluşturulurken bir hata oluştu.");
        }
    }
}