using FPP.Application.DTOs.LabEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPP.Application.Interface.IServices
{
    public interface IEventParticipantService
    {
        Task<int> CountUpcomingBookingsForUserAsync(int userId);
        Task<Dictionary<int, List<BookingCalendarItem>>> GetUserBookingsGroupedByDayAsync(int userId, int year, int month);
    }
}
