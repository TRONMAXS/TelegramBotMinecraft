using Microsoft.Win32;
using System.Diagnostics;

namespace TelegramBotMinecraft.Core.Services
{
    public class StartupManager
    {
        private readonly string AppName = AppDomain.CurrentDomain.FriendlyName;
        private readonly string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public void EnableStartup()
        {
            if (IsStartupEnabled()) return;

            using var key = Registry.CurrentUser.CreateSubKey(RunKey, true);

            var exePath = Process.GetCurrentProcess().MainModule?.FileName;

            if (exePath != null)
            {
                key?.SetValue(AppName, $"\"{exePath}\"");
            }
        }

        public void DisableStartup()
        {
            if (!IsStartupEnabled()) return;

            using var key = Registry.CurrentUser.OpenSubKey(RunKey, true);
            key?.DeleteValue(AppName, false);
        }
        public bool IsStartupEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, false);
            return key?.GetValue(AppName) != null;
        }

    }
}
