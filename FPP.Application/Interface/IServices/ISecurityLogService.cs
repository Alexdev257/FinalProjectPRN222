using FPP.Application.DTOs.SecurityLog;
using FPP.Domain.Entities;

namespace FPP.Application.Interface.IServices
{
    public interface ISecurityLogService
    {
        Task<SecurityLogResponse> CreateSecurityLogAsync(SecurityLogRequest securityLogRequest);
        Task<IEnumerable<SecurityLogResponse>> GetAllSecurityLogsAsync(int pageNumber, int pageSize, int labEventId);
        Task<SecurityLogResponse> GetSecurityLogByIdAsync(int logId);
        Task<SecurityLogResponse> UpdateSecurityLogAsync(int logId, SecurityLogRequest securityLogRequest);
        Task<bool> DeleteSecurityLogAsync(int logId);
        Task<IEnumerable<LabEvent>> GetAllLabEventsAsync();
        Task<LabEvent> GetLabEventByIdAsync(int eventId);
        Task<bool> AcknowledgeSecurityLogAsync(int logId, int managerId, string note);
        Task<IEnumerable<SecurityLogResponse>> GetSecurityLogsBySecurityIdAsync(int securityId);
    }
}
