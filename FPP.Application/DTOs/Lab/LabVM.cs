using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.DTOs.Lab
{
    public class LabVM
    {
        public int LabId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public bool IsAvailableNow { get; set; } = true;
    }
}
