using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.DTOs.LabEvent
{
    public class BookingCalendarItem
    {
        public int EventId { get; set; }
        public string LabName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
    }
}
