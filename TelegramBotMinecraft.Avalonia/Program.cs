using Avalonia;
using System;
using System.Threading;

namespace TelegramBotMinecraft.Avalonia;

class Program
{
    private static Mutex? _mutex;
    private const string MutexName = "TRONMAXS.TelegramBotMinecraft.SingleInstanceMutex";

    [STAThread]
    public static void Main(string[] args)
    {
        _mutex = new Mutex(true, MutexName, out bool isNewInstance);

        if (!isNewInstance) return;

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        finally { _mutex.ReleaseMutex(); }

    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
