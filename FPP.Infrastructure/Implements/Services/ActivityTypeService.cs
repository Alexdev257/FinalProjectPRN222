using FPP.Application.Interface.IRepositories;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Infrastructure.Implements.Services
{
    public class ActivityTypeService : IActivityTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivityTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<ActivityType>> GetAllActivityTypesAsync()
        {
            return await _unitOfWork.ActivityTypes.GetAllAsync()
                        .OrderBy(at => at.Name)
                        .ToListAsync();
        }
    }
}
