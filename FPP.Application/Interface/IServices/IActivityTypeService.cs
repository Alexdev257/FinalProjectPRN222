using FPP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IServices
{
    public interface IActivityTypeService
    {
        Task<List<ActivityType>> GetAllActivityTypesAsync();
    }
}
