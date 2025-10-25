using FPP.Application.DTOs.Lab;
using FPP.Domain.Entities;
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
        Task<List<LabZone>> GetZonesByLabIdAsync(int labId);
        Task<List<Lab>> GetAllLabsAsync();

    }
}
