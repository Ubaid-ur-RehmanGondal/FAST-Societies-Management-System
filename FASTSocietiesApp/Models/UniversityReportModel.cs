namespace FASTSocietiesApp.Models
{
    /// <summary>
    /// Custom DTO representing the university-wide statistics
    /// returned by the Admin features.
    /// </summary>
    public class UniversityReportModel
    {
        public int TotalStudents { get; set; }
        public int TotalActiveSocieties { get; set; }
        public int TotalEvents { get; set; }
    }
}
