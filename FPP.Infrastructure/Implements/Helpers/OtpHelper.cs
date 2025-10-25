using FPP.Application.Interface.IHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Infrastructure.Implements.Helpers
{
    public class OtpHelper : IOtpHelper
    {
        public string GenerateOtpAsync(int length)
        {
            var random = new Random();
            var otp = new string(Enumerable.Range(0, length).Select(_ => (char)('0' + random.Next(10))).ToArray());
            return otp;
        }
    }
}
