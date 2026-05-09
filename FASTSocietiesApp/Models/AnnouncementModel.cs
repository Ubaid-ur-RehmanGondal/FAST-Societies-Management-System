namespace FASTSocietiesApp.Models
{
    /// <summary>
    /// Represents an Announcement posted by a Society Head.
    /// Maps to the Announcements table.
    /// </summary>
    public class AnnouncementModel
    {
        public int AnnouncementId { get; set; }
        public int SocietyId { get; set; }
        public int PostedByUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }

        // UI Convenience properties joined from Users table
        public string PostedByFullName { get; set; } = string.Empty;
    }
}
