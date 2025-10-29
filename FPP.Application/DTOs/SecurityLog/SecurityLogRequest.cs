namespace FPP.Application.DTOs.SecurityLog
{
    public class SecurityLogRequest
    {
        public required int SecurityId { get; set; }
        public required int EventId { get; set; }

        public required string Action { get; set; }

        public string? PhotoUrl { get; set; }

        public string? Notes { get; set; }
    }
}
