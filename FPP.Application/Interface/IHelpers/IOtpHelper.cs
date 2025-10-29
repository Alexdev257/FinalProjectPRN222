using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IHelpers
{
    public interface IOtpHelper
    {
        string GenerateOtpAsync(int length);
    }
}
