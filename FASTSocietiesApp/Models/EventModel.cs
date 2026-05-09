namespace FASTSocietiesApp.Models
{
    /// <summary>
    /// Represents an Event created by a society. Maps to Events table.
    /// </summary>
    public class EventModel
    {
        public int EventId { get; set; }
        public int SocietyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string? Venue { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }

        // Added for UI display convenience (joined from Societies)
        public string SocietyName { get; set; } = string.Empty;
    }
}
