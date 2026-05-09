namespace FASTSocietiesApp.Models
{
    /// <summary>
    /// Represents a Task assigned to a society member.
    /// Maps to the Tasks table.
    /// </summary>
    public class TaskModel
    {
        public int TaskId { get; set; }
        public int SocietyId { get; set; }
        public int AssignedToUserId { get; set; }
        public int AssignedByUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }

        // UI Convenience properties joined from Users table
        public string AssignedToFullName { get; set; } = string.Empty;
        public string AssignedByFullName { get; set; } = string.Empty;
    }
}
