namespace FASTSocietiesApp.Helpers
{
    /// <summary>
    /// Static, application-wide session container. Holds identity of the
    /// currently logged-in user. All forms must consult this before loading.
    /// </summary>
    public static class SessionManager
    {
        public static int LoggedInUserId { get; private set; }
        public static UserRole LoggedInUserRole { get; private set; } = UserRole.None;
        public static string LoggedInUserName { get; private set; } = string.Empty;

        /// <summary>
        /// Returns true when a user has been authenticated in this session.
        /// </summary>
        public static bool IsLoggedIn => LoggedInUserId > 0 && LoggedInUserRole != UserRole.None;

        /// <summary>
        /// Records the authenticated user for the lifetime of the process.
        /// </summary>
        public static void StartSession(int userId, UserRole role, string fullName)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));
            if (role == UserRole.None)
                throw new ArgumentException("Role must be set.", nameof(role));

            LoggedInUserId = userId;
            LoggedInUserRole = role;
            LoggedInUserName = fullName ?? string.Empty;
        }

        /// <summary>
        /// Clears all session data. Call on logout.
        /// </summary>
        public static void EndSession()
        {
            LoggedInUserId = 0;
            LoggedInUserRole = UserRole.None;
            LoggedInUserName = string.Empty;
        }
    }
}
