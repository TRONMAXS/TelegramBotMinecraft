using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;


namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class JavaArgumentManagerViewModel : ObservableObject
    {
        private readonly ServerRepository? _serverRepository;

        private Server? editableServer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FinalArguments))]
        private string _jarFileName = "server.jar";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FinalArguments))]
        [NotifyPropertyChangedFor(nameof(MemoryString))]
        private int _memoryMb = 4096;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FinalArguments))]
        [NotifyPropertyChangedFor(nameof(IsMemorySliderVisible))]
        [NotifyPropertyChangedFor(nameof(IsCustomArgsVisible))]
        [NotifyPropertyChangedFor(nameof(IsFullCustomLineVisible))]
        private int _presetIndex = 1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FinalArguments))]
        private string? _customArguments;

        public bool IsCustomArgsEnabled => PresetIndex == 2;
        public bool IsMemorySliderVisible => PresetIndex != 3;
        public bool IsCustomArgsVisible => PresetIndex == 2;
        public bool IsFullCustomLineVisible => PresetIndex == 3;

        public string FinalArguments
        {
            get
            {
                if (PresetIndex == 3)
                {
                    return CustomArguments;
                }

                string memoryFlags = $"-Xms{MemoryMb}M -Xmx{MemoryMb}M";
                string jarFile = string.IsNullOrWhiteSpace(JarFileName) ? "server.jar" : JarFileName;

                if (PresetIndex == 0)
                {
                    return $"{memoryFlags} -jar {jarFile} nogui";
                }
                if (PresetIndex == 1)
                {
                    string aikarFlags = "--add-modules=jdk.incubator.vector -XX:+UseG1GC -XX:+ParallelRefProcEnabled -XX:MaxGCPauseMillis=200 -XX:+UnlockExperimentalVMOptions -XX:+DisableExplicitGC -XX:+AlwaysPreTouch -XX:G1HeapWastePercent=5 -XX:G1MixedGCCountTarget=4 -XX:InitiatingHeapOccupancyPercent=15 -XX:G1MixedGCLiveThresholdPercent=90 -XX:G1RSetUpdatingPauseTimePercent=5 -XX:SurvivorRatio=32 -XX:+PerfDisableSharedMem -XX:MaxTenuringThreshold=1 -Dusing.aikars.flags=https://mcflags.emc.gs -Daikars.new.flags=true -XX:G1NewSizePercent=30 -XX:G1MaxNewSizePercent=40 -XX:G1HeapRegionSize=8M -XX:G1ReservePercent=20";
                    return $"{memoryFlags} {aikarFlags} -jar {jarFile} nogui";
                }

                return $"{memoryFlags} {CustomArguments} -jar {jarFile} nogui";
            }
        }

        public string MemoryString
        {
            get
            {
                if (MemoryMb >= 1024 && MemoryMb % 1024 == 0)
                    return $"{MemoryMb / 1024} ГБ";
                return $"{MemoryMb} МБ";
            }
        }

        public JavaArgumentManagerViewModel(ServerRepository serverRepository, Server? selectedServer)
        {
            _serverRepository = serverRepository;
            editableServer = selectedServer;
            LoadJavaArgumentsAsync(editableServer);
        }


        private async void LoadJavaArgumentsAsync(Server? Server)
        {
            string? javaArgs = Server?.JavaArgs;
            if (string.IsNullOrWhiteSpace(javaArgs)) return;

            var minMatch = Regex.Match(javaArgs, @"-Xms(\d+)([MG])");
            if (minMatch.Success)
            {
                int value = int.Parse(minMatch.Groups[1].Value);
                string unit = minMatch.Groups[2].Value;
                MemoryMb = unit == "G" ? value * 1024 : value;
            }

            var maxMatch = Regex.Match(javaArgs, @"-Xmx(\d+)([MG])");
            if (maxMatch.Success)
            {
                int value = int.Parse(maxMatch.Groups[1].Value);
                string unit = maxMatch.Groups[2].Value;
                MemoryMb = unit == "G" ? value * 1024 : value;
            }

            var jarFileMatch = Regex.Match(javaArgs, @"-jar\s+(.*?\.jar)");
            if (jarFileMatch.Success)
            {
                JarFileName = jarFileMatch.Groups[1].Value;
            }


            if (!javaArgs.Contains("-Xms") && !javaArgs.Contains("-Xmx") && !javaArgs.Contains("-jar"))
            {
                PresetIndex = 3;
                CustomArguments = javaArgs;
            }
            else if (javaArgs.Contains("-XX:+UseG1GC") && javaArgs.Contains("G1NewSizePercent"))
            {
                PresetIndex = 1;
            }
            else if (IsOnlyStandardArgs(javaArgs))
            {
                PresetIndex = 0;
            }
            else
            {
                PresetIndex = 2;
                string cleanArgs = Regex.Replace(javaArgs, @"-Xm[sx]\d+[MG]", "");
                cleanArgs = Regex.Replace(cleanArgs, @"-jar\s+.*?\.jar", "");
                cleanArgs = cleanArgs.Replace("nogui", "");

                CustomArguments = cleanArgs.Trim();
            }
        }
        private bool IsOnlyStandardArgs(string javaArgs)
        {
            string clean = Regex.Replace(javaArgs, @"-Xm[sx]\d+[MG]", "");
            clean = Regex.Replace(clean, @"-jar\s+.*?\.jar", "");
            clean = clean.Replace("nogui", "").Trim();
            return string.IsNullOrWhiteSpace(clean);
        }


        [RelayCommand]
        private async Task Save()
        {
            CloseWindowWithResult(FinalArguments);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseWindowWithResult(editableServer.JavaArgs);
        }

        private void CloseWindowWithResult(string result)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var currentWindow = desktop.Windows.FirstOrDefault(w => w.DataContext == this);

                currentWindow?.Close(result);
            }
        }
    }
}
