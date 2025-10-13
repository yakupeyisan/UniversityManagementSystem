using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ScheduleEvents;


/// <summary>
/// Ders programı yayınlandığında tetiklenen event
/// </summary>
public class SchedulePublishedEvent : BaseDomainEvent
{
    public Guid ScheduleId { get; }
    public string AcademicYear { get; }
    public int Semester { get; }

    public SchedulePublishedEvent(Guid scheduleId, string academicYear, int semester)
    {
        ScheduleId = scheduleId;
        AcademicYear = academicYear;
        Semester = semester;
    }
}