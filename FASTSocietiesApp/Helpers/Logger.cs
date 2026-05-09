namespace FASTSocietiesApp.Helpers
{
    /// <summary>
    /// Lightweight static logger that appends entries to log.txt next to
    /// the running executable. Thread-safe via a private lock object.
    /// </summary>
    public static class Logger
    {
        private static readonly object _lock = new object();
        private static readonly string _logFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

        /// <summary>
        /// Writes an informational entry to the log file.
        /// </summary>
        public static void Info(string message)
        {
            Write("INFO", message, null);
        }

        /// <summary>
        /// Writes an error entry. Includes exception details when supplied.
        /// </summary>
        public static void Error(string message, Exception? ex = null)
        {
            Write("ERROR", message, ex);
        }

        private static void Write(string level, string message, Exception? ex)
        {
            try
            {
                lock (_lock)
                {
                    using StreamWriter writer = new StreamWriter(_logFilePath, append: true);
                    writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
                    if (ex != null)
                    {
                        writer.WriteLine($"    Exception: {ex.GetType().FullName}: {ex.Message}");
                        writer.WriteLine($"    Stack: {ex.StackTrace}");
                    }
                }
            }
            catch
            {
                // Logging must never crash the app.
            }
        }
    }
}
