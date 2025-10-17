using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.DTOs
{
    public class RegisterRequest
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password　{ get; set; } = null!;
    }
}
