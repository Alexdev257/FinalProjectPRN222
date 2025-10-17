using System;
using System.Collections.Generic;

namespace FPP.Domain.Entities;

public partial class EventParticipant
{
    public int EventId { get; set; }

    public int UserId { get; set; }

    public decimal Role { get; set; }

    public virtual LabEvent Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
