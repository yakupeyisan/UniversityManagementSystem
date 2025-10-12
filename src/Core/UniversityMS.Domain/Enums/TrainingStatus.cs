namespace UniversityMS.Domain.Enums;

public enum TrainingStatus
{
    Planned = 1,
    Registration = 2,     // Kayıt açık
    InProgress = 3,
    Completed = 4,
    Cancelled = 5,
    Postponed = 6,
    RegistrationOpen = 7
}