using System;
using System.Collections.Generic;

namespace FPP.Domain.Entities;

public partial class ActivityType
{
    public int ActivityTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<LabEvent> LabEvents { get; set; } = new List<LabEvent>();
}
