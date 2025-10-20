using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.ProcurementFeature.Commands;

public record SubmitPurchaseRequestCommand(Guid RequestId) : IRequest<Result<Guid>>;