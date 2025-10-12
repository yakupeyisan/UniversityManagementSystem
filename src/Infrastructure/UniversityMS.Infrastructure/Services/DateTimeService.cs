﻿using UniversityMS.Application.Common.Interfaces;

namespace UniversityMS.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}