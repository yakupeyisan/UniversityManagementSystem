using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.ProcurementFeature.Commands;

public record ApprovePurchaseRequestCommand(Guid RequestId) : IRequest<Result<Guid>>;