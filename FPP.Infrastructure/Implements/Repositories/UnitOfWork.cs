using FPP.Application.Interface.IRepositories;
using FPP.Domain.Entities;
using FPP.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Infrastructure.Implements.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public IBaseRepository<ActivityType> ActivityTypes => new BaseRepository<ActivityType>(_context);

        public IBaseRepository<EventParticipant> EventParticipants => new BaseRepository<EventParticipant>(_context);

        public IBaseRepository<Lab> Labs => new BaseRepository<Lab>(_context);

        public IBaseRepository<LabEvent> LabEvents => new BaseRepository<LabEvent>(_context);

        public IBaseRepository<LabZone> LabZones => new BaseRepository<LabZone>(_context);

        public IBaseRepository<Notification> Notifications => new BaseRepository<Notification>(_context);

        public IBaseRepository<Report> Reports => new BaseRepository<Report>(_context);

        public IBaseRepository<SecurityLog> SecurityLogs => new BaseRepository<SecurityLog>(_context);

        public IBaseRepository<User> Users => new BaseRepository<User>(_context);

        public async Task<bool> CompleteAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
