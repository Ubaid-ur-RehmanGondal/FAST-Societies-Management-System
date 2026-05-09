namespace FASTSocietiesApp.Models
{
    /// <summary>
    /// Data transfer object that mirrors a row in the Societies table,
    /// optionally enriched with the head's full name when joined with Users.
    /// Property names and types match database_schema.sql exactly.
    /// </summary>
    public class SocietyModel
    {
        /// <summary>Societies.SocietyId, IDENTITY primary key.</summary>
        public int SocietyId { get; set; }

        /// <summary>Societies.Name, NVARCHAR(100), NOT NULL, UNIQUE.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Societies.Description, NVARCHAR(500), nullable.</summary>
        public string? Description { get; set; }

        /// <summary>Societies.HeadUserId, FK to Users.UserId.</summary>
        public int HeadUserId { get; set; }

        /// <summary>
        /// Optional, populated only by queries that JOIN Users.
        /// Empty when the row was loaded without join.
        /// </summary>
        public string HeadFullName { get; set; } = string.Empty;

        /// <summary>
        /// Societies.Status, constrained to:
        /// 'Pending', 'Active', 'Suspended', 'Deleted'.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Societies.CreatedAt, DATETIME, defaulted by DB.</summary>
        public DateTime CreatedAt { get; set; }
    }
}
