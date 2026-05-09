namespace FASTSocietiesApp.Helpers
{
    /// <summary>
    /// Defines the three roles supported by the system, matching the
    /// CHECK constraint on the Users.Role column in the database.
    /// </summary>
    public enum UserRole
    {
        None = 0,
        Student = 1,
        SocietyHead = 2,
        Admin = 3
    }
}
