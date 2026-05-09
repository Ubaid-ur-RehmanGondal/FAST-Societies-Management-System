namespace FASTSocietiesApp.Helpers
{
    /// <summary>
    /// Custom application exception. DAL wraps low-level errors in this type
    /// and BLL adds context before re-throwing. Forms display the Message
    /// via MessageBox without exposing stack traces.
    /// </summary>
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }

        public AppException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
