using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IHelpers
{
    public interface IEmailHelper
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string templateHtml, Dictionary<string, string> placeholders);
    }
}
