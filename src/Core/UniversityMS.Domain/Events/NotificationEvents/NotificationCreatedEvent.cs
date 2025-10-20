using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.NotificationEvents;

/// <summary>
/// Bildirim oluşturulduğunda tetiklenen event
/// </summary>
public class NotificationCreatedEvent : BaseDomainEvent
{
    public Guid NotificationId { get; }
    public Guid RecipientId { get; }
    public NotificationType Type { get; }
    public NotificationPriority Priority { get; }
    public string Title { get; }

    public NotificationCreatedEvent(Guid notificationId, Guid recipientId, NotificationType type, NotificationPriority priority, string title)
    {
        NotificationId = notificationId;
        RecipientId = recipientId;
        Type = type;
        Priority = priority;
        Title = title;
    }
}