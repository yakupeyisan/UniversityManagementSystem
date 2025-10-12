﻿namespace UniversityMS.Domain.Entities.Common;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }

    void Delete(string? deletedBy = null);
    void Restore();
}