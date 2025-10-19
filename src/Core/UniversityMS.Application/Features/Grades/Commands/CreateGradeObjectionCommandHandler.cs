using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Grades.Commands;

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

    public async Task<Result<Guid>> Handle(CreateGradeObjectionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verify grade exists
            var grade = await _gradeRepository.GetByIdAsync(request.GradeId, cancellationToken);
            if (grade == null)
                return Result<Guid>.Failure("Not kaydı bulunamadı.");

            // Verify student owns the grade
            if (grade.StudentId != request.StudentId)
                return Result<Guid>.Failure("Bu nota itiraz etme yetkiniz yok.");

            // ✅ CHECK FOR EXISTING OBJECTION:
            var existingObjection = await _objectionRepository.FirstOrDefaultAsync(
                o => o.GradeId == request.GradeId && o.Status == ObjectionStatus.Pending,
                cancellationToken);

            if (existingObjection != null)
                return Result<Guid>.Failure("Bu not için zaten bekleyen bir itiraz var.");

            // ✅ CREATE OBJECTION:
            var objection = GradeObjection.Create(
                request.GradeId,
                request.StudentId,
                request.CourseId,
                request.Reason
            );

            await _objectionRepository.AddAsync(objection, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Grade objection created. ObjectionId: {ObjectionId}, GradeId: {GradeId}",
                objection.Id, request.GradeId);

            return Result<Guid>.Success(objection.Id, "Not itirazı başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating grade objection");
            return Result<Guid>.Failure("Not itirazı oluşturulurken bir hata oluştu. Hata: " + ex.Message);
        }
    }
}