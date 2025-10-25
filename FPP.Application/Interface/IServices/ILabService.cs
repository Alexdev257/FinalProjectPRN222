using FPP.Application.DTOs.Lab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IServices
{
    public interface ILabService
    {
        Task<List<LabVM>> GetAllLabsWithAvailabilityAsync();
        int GetTotalLabCountAsync();
        Task<int> GetAvailableLabCountAsync();
    }
}
