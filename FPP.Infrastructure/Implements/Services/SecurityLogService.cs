using FPP.Application.DTOs.SecurityLog;
using FPP.Application.Interface.IRepositories;
using FPP.Application.Interface.IServices;
using FPP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace FPP.Infrastructure.Implements.Services
{
    public class SecurityLogService : ISecurityLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SecurityLogService> _logger;

        public SecurityLogService(IUnitOfWork unitOfWork, ILogger<SecurityLogService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<SecurityLogResponse> CreateSecurityLogAsync(SecurityLogRequest securityLogRequest)
        {
            var userSecurity = await _unitOfWork.Users.GetByIdAsync(securityLogRequest.SecurityId);

            if (userSecurity == null)
            {
                _logger.LogError("Security user with ID {SecurityId} not found.", securityLogRequest.SecurityId);
                throw new Exception("Security user not found.");
            }

            var securityLog = new SecurityLog
            {
                EventId = securityLogRequest.EventId,
                SecurityId = securityLogRequest.SecurityId,
                Action = securityLogRequest.Action,
                Timestamp = DateTime.UtcNow,
                PhotoUrl = securityLogRequest.PhotoUrl,
                Notes = securityLogRequest.Notes
            };
            _unitOfWork.SecurityLogs.Add(securityLog);
            await _unitOfWork.CompleteAsync();

            var log = await _unitOfWork.SecurityLogs.Query()
                    .Where(sl => sl.LogId == securityLog.LogId)
                    .Include(sl => sl.Event.Lab)
                    .Include(sl => sl.Event.Zone)
                    .Include(sl => sl.Event.Organizer)
                    .FirstOrDefaultAsync();

            _logger.LogInformation("Security log created with ID {LogId} by Security User ID {SecurityId}.", log.LogId, log.SecurityId);

            return new SecurityLogResponse
            {
                LogId = log.LogId,
                EventId = log.EventId,
                SecurityId = log.SecurityId,
                Action = log.Action,
                Timestamp = log.Timestamp,
                PhotoUrl = log.PhotoUrl,
                Notes = log.Notes,
                LabName = log.Event?.Lab?.Name,
                ZoneName = log.Event?.Zone?.Name,
                OrganizerName = log.Event?.Organizer?.Name,
                OrganizerEmail = log.Event?.Organizer?.Email
            };

        }

        public Task<bool> DeleteSecurityLogAsync(int logId)
        {
            var securityLog = _unitOfWork.SecurityLogs.GetByIdAsync(logId).Result;

            if (securityLog == null)
            {
                _logger.LogError("Security log with ID {LogId} not found for deletion.", logId);
                throw new Exception("Security log not found.");
            }

            _unitOfWork.SecurityLogs.Remove(securityLog);
            _unitOfWork.CompleteAsync().Wait();
            _logger.LogInformation("Security log with ID {LogId} deleted successfully.", logId);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<LabEvent>> GetAllLabEventsAsync()
        {
            var labEvents = _unitOfWork.LabEvents.Query()
                .Include(le => le.Lab)
                .Include(le => le.Zone)
                .Include(le => le.Organizer)
                .Include(le => le.ActivityType)
                .OrderByDescending(le => le.Lab.Name)
                .OrderByDescending(le => le.StartTime)
                .ToListAsync();
            _logger.LogInformation("Retrieved all lab events.");
            return Task.FromResult(labEvents.Result.AsEnumerable());
        }

        public async Task<IEnumerable<SecurityLogResponse>> GetAllSecurityLogsAsync(
           int pageNumber, int pageSize, int labEventId)
        {
            var securityLogs = await _unitOfWork.SecurityLogs.Query()
                .Include(sl => sl.Event.Lab)
                .Include(sl => sl.Event.Zone)
                .Include(sl => sl.Event.Organizer) 
                .Where(sl => sl.EventId == labEventId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Retrieved page {PageNumber} of security logs with page size {PageSize}.",
                pageNumber, pageSize);

            return securityLogs.Select(securityLog => new SecurityLogResponse
            {
                LogId = securityLog.LogId,
                EventId = securityLog.EventId,
                SecurityId = securityLog.SecurityId,
                Action = securityLog.Action,
                Timestamp = securityLog.Timestamp,
                PhotoUrl = securityLog.PhotoUrl,
                Notes = securityLog.Notes,
                LabName = securityLog.Event?.Lab?.Name,
                ZoneName = securityLog.Event?.Zone?.Name,
                OrganizerName = securityLog.Event?.Organizer?.Name,
                OrganizerEmail = securityLog.Event?.Organizer?.Email
            });
        }


        public Task<LabEvent> GetLabEventByIdAsync(int eventId)
        {
            var labEvent = _unitOfWork.LabEvents.Query()
                .Include(le => le.Lab)
                .Include(le => le.Zone)
                .Include(le => le.Organizer)
                .Include(le => le.ActivityType)
                .FirstOrDefaultAsync(le => le.EventId == eventId);
            _logger.LogInformation("Retrieved lab event with ID {EventId}.", eventId);

            if (labEvent.Result == null)
            {
                _logger.LogError("Lab event with ID {EventId} not found.", eventId);
                return Task.FromResult<LabEvent>(null);
            }

            return Task.FromResult(labEvent.Result);
        }

        public Task<SecurityLogResponse> GetSecurityLogByIdAsync(int logId)
        {

            var securityLog = _unitOfWork.SecurityLogs.Query()
               .Where(sl => sl.LogId == logId)
               .Include(sl => sl.Event.Lab)
               .Include(sl => sl.Event.Zone)
               .Include(sl => sl.Security)
               .FirstOrDefaultAsync();

            if (securityLog.Result == null)
            {
                _logger.LogError("Security log with ID {LogId} not found.", logId);
                throw new Exception("Security log not found.");
            }
            return Task.FromResult(new SecurityLogResponse
            {
                LogId = securityLog.Result.LogId,
                EventId = securityLog.Result.EventId,
                SecurityId = securityLog.Result.SecurityId,
                Action = securityLog.Result.Action,
                Timestamp = securityLog.Result.Timestamp,
                PhotoUrl = securityLog.Result.PhotoUrl,
                Notes = securityLog.Result.Notes,
                LabName = securityLog.Result.Event.Lab.Name,
                ZoneName = securityLog.Result.Event.Zone.Name,
                OrganizerName = securityLog.Result.Event.Organizer.Name,
                OrganizerEmail = securityLog.Result.Event.Organizer.Email
            });
        }

        public Task<SecurityLogResponse> UpdateSecurityLogAsync(int logId, SecurityLogRequest securityLogRequest)
        {
            var securityLog = _unitOfWork.SecurityLogs.GetByIdAsync(logId).Result;

            if (securityLog == null)
            {
                _logger.LogError("Security log with ID {LogId} not found for update.", logId);
                throw new Exception("Security log not found.");
            }

            securityLog.EventId = securityLogRequest.EventId;
            securityLog.SecurityId = securityLogRequest.SecurityId;
            securityLog.Action = securityLogRequest.Action;
            securityLog.PhotoUrl = securityLogRequest.PhotoUrl;
            securityLog.Notes = securityLogRequest.Notes;

            _unitOfWork.SecurityLogs.Update(securityLog);
            _unitOfWork.CompleteAsync().Wait();
            _logger.LogInformation("Security log with ID {LogId} updated successfully.", logId);

            return Task.FromResult(new SecurityLogResponse
            {
                LogId = securityLog.LogId,
                EventId = securityLog.EventId,
                SecurityId = securityLog.SecurityId,
                Action = securityLog.Action,
                Timestamp = securityLog.Timestamp,
                PhotoUrl = securityLog.PhotoUrl,
                Notes = securityLog.Notes
            });
        }
    }
}
