using FASTSocietiesApp.Forms.Shared;
using FASTSocietiesApp.Helpers;

namespace FASTSocietiesApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.ThreadException += (sender, args) =>
            {
                Logger.Error("Unhandled UI thread exception.", args.Exception);
                MessageBox.Show(
                    "An unexpected error occurred. Please try again.",
                    "FAST Societies",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                {
                    Logger.Error("Unhandled non-UI exception.", ex);
                }
            };

            Logger.Info("Application starting.");

            Application.Run(new LoginForm());
        }
    }
}
