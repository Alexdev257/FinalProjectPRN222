using System;
using System.Collections.Generic;

namespace FPP.Domain.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public decimal Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    public virtual ICollection<LabEvent> LabEvents { get; set; } = new List<LabEvent>();

    public virtual ICollection<Lab> Labs { get; set; } = new List<Lab>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<SecurityLog> SecurityLogs { get; set; } = new List<SecurityLog>();
}
