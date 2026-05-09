namespace FASTSocietiesApp.Models
{
    /// <summary>
    /// Data transfer object that mirrors a row in the Memberships table,
    /// optionally enriched with display fields (society name, applicant name)
    /// for grids that render joins.
    /// </summary>
    public class MembershipModel
    {
        /// <summary>Memberships.MembershipId, IDENTITY primary key.</summary>
        public int MembershipId { get; set; }

        /// <summary>Memberships.UserId, FK to Users.UserId (Student role).</summary>
        public int UserId { get; set; }

        /// <summary>Memberships.SocietyId, FK to Societies.SocietyId.</summary>
        public int SocietyId { get; set; }

        /// <summary>
        /// Memberships.Status, constrained to:
        /// 'Pending', 'Approved', 'Rejected'. Defaults to 'Pending' on insert.
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>Memberships.AppliedAt, DATETIME, defaulted by DB.</summary>
        public DateTime AppliedAt { get; set; }

        /// <summary>Memberships.UpdatedAt, DATETIME, nullable.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Optional joined value: Societies.Name.</summary>
        public string SocietyName { get; set; } = string.Empty;

        /// <summary>Optional joined value: Users.FullName for the applicant.</summary>
        public string ApplicantFullName { get; set; } = string.Empty;
    }
}
