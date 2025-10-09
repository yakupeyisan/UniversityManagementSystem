﻿using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UniversityMS.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
