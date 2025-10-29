using FPP.Application.DTOs.Lab;
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
    public class LabService : ILabService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LabService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int GetTotalLabCountAsync()
        {
            return _unitOfWork.Labs.GetAllAsync().ToList().Count();
        }

        public async Task<int> GetAvailableLabCountAsync()
        {
            var now = DateTime.Now;
            // Lấy ID các lab đang bận từ LabEvents repository
            var busyLabIds = await _unitOfWork.LabEvents.GetAllAsync()
                .Where(e => e.StartTime <= now && e.EndTime > now && e.Status.ToLower() == "approved")
                .Select(e => e.LabId)
                .Distinct()
                .ToListAsync();

            var totalLabs = GetTotalLabCountAsync();
            return totalLabs - busyLabIds.Count;
        }

        public async Task<List<LabVM>> GetAllLabsWithAvailabilityAsync()
        {
            // Lấy tất cả labs
            var allLabs = await _unitOfWork.Labs.GetAllAsync()
                                        .OrderBy(l => l.Name) // Sắp xếp nếu muốn
                                        .ToListAsync();
            var now = DateTime.Now;

            // Lấy ID các lab đang bận
            var busyLabIds = await _unitOfWork.LabEvents.GetAllAsync()
                .Where(e => e.StartTime <= now && e.EndTime > now && e.Status.ToLower() == "approved")
                .Select(e => e.LabId)
                .Distinct()
                .ToListAsync();

            // Tạo ViewModel
            var labsViewModel = allLabs.Select(lab => new LabVM
            {
                LabId = lab.LabId,
                Name = lab.Name,
                Description = lab.Description,
                Location = lab.Location,
                IsAvailableNow = !busyLabIds.Contains(lab.LabId) // Kiểm tra ID có trong list bận không
            }).ToList();

            return labsViewModel;
        }

        public async Task<List<LabZone>> GetZonesByLabIdAsync(int labId)
        {
            return await _unitOfWork.LabZones.GetAllAsync()
                        .Where(z => z.LabId == labId)
                        .OrderBy(z => z.Name)
                        .ToListAsync();
        }

        public async Task<List<Lab>> GetAllLabsAsync()
        {
            // GetAllAsync() trả về IQueryable, cho phép thêm LINQ
            var query = _unitOfWork.Labs.GetAllAsync()
                                  .OrderBy(l => l.Name); // Sắp xếp theo tên Lab (tùy chọn)

            // Thực thi query và chuyển thành List bất đồng bộ
            return await query.ToListAsync();
        }

        public async Task<Lab?> GetLabByIdAsync(int labId)
        {
            return await _unitOfWork.Labs.GetByIdAsync(labId);
        }

        public async Task<LabZone?> GetZoneByIdAsync(int zoneId)
        {
            return await _unitOfWork.LabZones.GetByIdAsync(zoneId);
        }
    }
}
