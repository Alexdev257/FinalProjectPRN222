using FPP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<ActivityType> ActivityTypes { get; }
        IBaseRepository<EventParticipant> EventParticipants { get; }
        IBaseRepository<Lab> Labs { get; }
        IBaseRepository<LabEvent> LabEvents { get; }
        IBaseRepository<LabZone> LabZones { get; }
        IBaseRepository<Notification> Notifications { get; }
        IBaseRepository<Report> Reports { get; }
        IBaseRepository<SecurityLog> SecurityLogs { get; }
        IBaseRepository<User> Users{ get; }
        Task<bool> CompleteAsync();
    }
}
