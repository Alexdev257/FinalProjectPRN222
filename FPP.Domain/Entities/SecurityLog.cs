using System;
using System.Collections.Generic;

namespace FPP.Domain.Entities;

public partial class SecurityLog
{
    public int LogId { get; set; }

    public int EventId { get; set; }

    public int SecurityId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public string? PhotoUrl { get; set; }

    public string? Notes { get; set; }
    public string? Status { get; set; }

    public virtual LabEvent Event { get; set; } = null!;

    public virtual User Security { get; set; } = null!;
}
