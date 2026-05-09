namespace FASTSocietiesApp.Models
{
    /// <summary>
    /// Data transfer object that mirrors a row in the Users table.
    /// Used to pass user data between DAL, BLL, and Forms.
    /// Property names and types match database_schema.sql exactly.
    /// </summary>
    public class UserModel
    {
        /// <summary>Primary key from Users.UserId (IDENTITY).</summary>
        public int UserId { get; set; }

        /// <summary>Users.FullName, NVARCHAR(100), NOT NULL.</summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>Users.Email, NVARCHAR(100), NOT NULL, UNIQUE.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Users.PasswordHash, NVARCHAR(256). Always the hashed value;
        /// the plain-text password must never be stored on this object.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Users.Role, NVARCHAR(20). Constrained by DB to:
        /// 'Student', 'SocietyHead', 'Admin'.
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>Users.IsActive, BIT, NOT NULL, DEFAULT 1.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Users.CreatedAt, DATETIME, NOT NULL, DEFAULT GETDATE().</summary>
        public DateTime CreatedAt { get; set; }
    }
}
