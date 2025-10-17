using System;
using System.Collections.Generic;

namespace FPP.Domain.Entities;

public partial class Report
{
    public int ReportId { get; set; }

    public int? ZoneId { get; set; }

    public int? LabId { get; set; }

    public int GeneratedBy { get; set; }

    public string ReportType { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime GeneratedAt { get; set; }

    public virtual User GeneratedByNavigation { get; set; } = null!;

    public virtual Lab? Lab { get; set; }

    public virtual LabZone? Zone { get; set; }
}
