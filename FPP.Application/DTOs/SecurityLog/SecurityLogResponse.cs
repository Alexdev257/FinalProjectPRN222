namespace FPP.Application.DTOs.SecurityLog
{
    public class SecurityLogResponse
    {
        public int LogId { get; set; }

        public int EventId { get; set; }

        public int SecurityId { get; set; }

        public string Action { get; set; } = null!;

        public DateTime Timestamp { get; set; }

        public string? PhotoUrl { get; set; }

        public string? Notes { get; set; }

        public string? LabName { get; set; }

        public string? ZoneName { get; set; }

        public string? OrganizerName { get; set; }

        public string? OrganizerEmail { get; set; }
        public string? EventTitle { get; set; } = null!;
    }
}
