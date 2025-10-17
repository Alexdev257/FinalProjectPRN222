using System;
using System.Collections.Generic;

namespace FPP.Domain.Entities;

public partial class LabEvent
{
    public int EventId { get; set; }

    public int LabId { get; set; }

    public int ZoneId { get; set; }

    public int ActivityTypeId { get; set; }

    public int OrganizerId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ActivityType ActivityType { get; set; } = null!;

    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    public virtual Lab Lab { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual User Organizer { get; set; } = null!;

    public virtual ICollection<SecurityLog> SecurityLogs { get; set; } = new List<SecurityLog>();

    public virtual LabZone Zone { get; set; } = null!;
}
