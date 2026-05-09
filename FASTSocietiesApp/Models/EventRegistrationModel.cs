namespace FASTSocietiesApp.Models
{
    /// <summary>
    /// Represents a student's registration for an event. Maps to EventRegistrations.
    /// </summary>
    public class EventRegistrationModel
    {
        public int RegistrationId { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string TicketCode { get; set; } = string.Empty;

        // Added for UI display convenience (joined from Events)
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? Venue { get; set; }
    }
}
