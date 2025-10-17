using System;
using System.Collections.Generic;

namespace FPP.Domain.Entities;

public partial class LabZone
{
    public int ZoneId { get; set; }

    public int LabId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual Lab Lab { get; set; } = null!;

    public virtual ICollection<LabEvent> LabEvents { get; set; } = new List<LabEvent>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
