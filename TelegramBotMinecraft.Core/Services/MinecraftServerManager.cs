using CoreRCON;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using TelegramBotMinecraft.Core.Database;
using static TelegramBotMinecraft.Core.Models.ServerStatusModel;

namespace TelegramBotMinecraft.Core.Services
{
    public class MinecraftServerManager
    {
        private readonly JavaRepository _javaRepository;
        private readonly ServerRepository _serverRepository;

        private readonly ConcurrentDictionary<string, Process> _activeProcesses = new();
        private readonly ConcurrentDictionary<string, ServerStatus> _currentStatuses = new();

        public event Action<string, ServerStatus>? ServerStatusChanged;

        private readonly Timer _backgroundCheckTimer;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5);

        private readonly Regex startingRegex = new(@"Starting minecraft server", RegexOptions.Compiled);
        private readonly Regex doneRegex = new(@"Done \(.*?\)!", RegexOptions.Compiled);
        private readonly Regex stoppingRegex = new(@"Stopping the server", RegexOptions.Compiled);
        private readonly Regex warningRegex = new(@"Can't keep up!", RegexOptions.Compiled);


        public MinecraftServerManager(JavaRepository javaRepository, ServerRepository serverRepository)
        {
            _javaRepository = javaRepository;
            _serverRepository = serverRepository;

            _backgroundCheckTimer = new Timer(async _ => await ExecutionRecoveryCheckAsync(), null, TimeSpan.Zero, _checkInterval);
        }

        public ServerStatus GetServerStatus(string serverName)
        {
            return _currentStatuses.TryGetValue(serverName, out var status) ? status : ServerStatus.Offline;
        }

        private async Task ExecutionRecoveryCheckAsync()
        {
            var servers = await _serverRepository.GetAllServers();
            if (servers == null) return;

            foreach (var server in servers)
            {
                var processStatus = CheckProcessByPid(server.IdProcess);

                ServerStatus finalStatus = ServerStatus.Offline;

                if (processStatus == ServerStatus.Online)
                {
                    if (!_activeProcesses.ContainsKey(server.Name))
                    {
                        try
                        {
                            var existingProcess = Process.GetProcessById(server.IdProcess ?? 0);

                            existingProcess.EnableRaisingEvents = true;
                            existingProcess.Exited += async (s, e) => await HandleServerExit(server.Name);

                            _activeProcesses[server.Name] = existingProcess;
                        }
                        catch { }
                    }

                    var logStatus = await CheckLogTail(server.PathServer);
                    finalStatus = DetermineStatus(logStatus, processStatus);
                }
                else
                {
                    if (server.IdProcess != -1)
                    {
                        await _serverRepository.UpdateServer(server.Name, -1);
                        _activeProcesses.TryRemove(server.Name, out _);
                    }
                    finalStatus = ServerStatus.Offline;
                }

                if (!_currentStatuses.TryGetValue(server.Name, out var oldStatus) || oldStatus != finalStatus)
                {
                    ChangeStatus(server.Name, finalStatus);
                }
            }
        }

        public async Task<bool> StartServer(string serverName)
        {
            if (GetServerStatus(serverName) != ServerStatus.Offline) return false;

            var serverData = await _serverRepository.GetServerByName(serverName);
            if (serverData == null) return false;

            var javaData = await _javaRepository.GetJavaByName(serverData.JavaName);
            if (javaData == null) return false;

            try
            {
                ChangeStatus(serverName, ServerStatus.Starting);

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = javaData.Path,
                        WorkingDirectory = serverData.PathServer,
                        Arguments = serverData.JavaArgs,
                        CreateNoWindow = false,
                        RedirectStandardInput = false,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        UseShellExecute = false
                    }
                };

                process.EnableRaisingEvents = true;
                process.Exited += async (sender, e) => await HandleServerExit(serverName);

                process.Start();

                _activeProcesses[serverName] = process;
                await _serverRepository.UpdateServer(serverName, process.Id);

                return true;
            }
            catch (Exception)
            {
                ChangeStatus(serverName, ServerStatus.Offline);
                return false;
            }
        }

        public async Task<bool> StopServer(string serverName)
        {
            var serverData = await _serverRepository.GetServerByName(serverName);
            if (serverData == null) return false;

            if (GetServerStatus(serverName) != ServerStatus.Online) return false;

            try
            {
                if (serverData.RconEnable == 0) return false;

                using var rcon = new RCON(IPAddress.Parse("127.0.0.1"), Convert.ToUInt16(serverData.RconPort), serverData.RconPass);
                await rcon.SendCommandAsync("stop");

                ChangeStatus(serverName, ServerStatus.Stopping);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task SendCommand(string serverName, string command)
        {
            var serverData = await _serverRepository.GetServerByName(serverName);
            if (serverData == null) return;

            if (serverData.RconEnable == 0) return;

            using (var rcon = new RCON(IPAddress.Parse("127.0.0.1"), Convert.ToUInt16(serverData.RconPort), serverData.RconPass))
            {
                await rcon.SendCommandAsync(command);
            }

        }

        private async Task HandleServerExit(string serverName)
        {
            _activeProcesses.TryRemove(serverName, out _);
            await _serverRepository.UpdateServer(serverName, -1);
            ChangeStatus(serverName, ServerStatus.Offline);
        }

        private async Task<ServerStatus> CheckLogTail(string? pathServer)
        {
            string logFilePath = Path.Combine(pathServer ?? string.Empty, "logs", "latest.log");
            if (!File.Exists(logFilePath)) return ServerStatus.Offline;

            try
            {
                using var fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (fs.Length == 0) return ServerStatus.Offline;

                long seekPosition = Math.Max(0, fs.Length - 4000);
                fs.Seek(seekPosition, SeekOrigin.Begin);

                using var reader = new StreamReader(fs);
                var lines = new List<string>();
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }

                for (int i = lines.Count - 1; i >= 0; i--)
                {
                    string currentLine = lines[i];
                    if (stoppingRegex.IsMatch(currentLine)) return ServerStatus.Offline;
                    if (warningRegex.IsMatch(currentLine)) return ServerStatus.Warning;
                    if (doneRegex.IsMatch(currentLine)) return ServerStatus.Online;
                    if (startingRegex.IsMatch(currentLine)) return ServerStatus.Starting;
                }

                return ServerStatus.Offline;
            }
            catch
            {
                return ServerStatus.Offline;
            }
        }

        private void ChangeStatus(string serverName, ServerStatus newStatus)
        {
            _currentStatuses[serverName] = newStatus;
            ServerStatusChanged?.Invoke(serverName, newStatus);
        }

        private ServerStatus CheckProcessByPid(int? pid)
        {
            if (pid == null || pid <= 0) return ServerStatus.Offline; 
            try 
            { 
                Process process = Process.GetProcessById(pid.Value); 
                if (!process.HasExited) 
                { 
                    string procName = process.ProcessName.ToLower(); 
                    if (procName.Contains("java") || procName.Contains("javaw")) return ServerStatus.Online; 
                } return ServerStatus.Offline; 
            } 
            catch (ArgumentException) 
            { 
                return ServerStatus.Offline; 
            }
            catch 
            { 
                return ServerStatus.Offline; 
            }
        }

        private ServerStatus DetermineStatus(ServerStatus logStatus, ServerStatus processStatus) 
        {
            if (processStatus == ServerStatus.Online && logStatus == ServerStatus.Online) return ServerStatus.Online; 
            if (processStatus == ServerStatus.Online && logStatus == ServerStatus.Warning) return ServerStatus.Warning; 
            if (processStatus == ServerStatus.Online && logStatus == ServerStatus.Starting) return ServerStatus.Starting; 
            if (processStatus == ServerStatus.Online && logStatus == ServerStatus.Offline) return ServerStatus.Starting; 
            return ServerStatus.Offline; 
        }

        public void Dispose()
        {
            _backgroundCheckTimer?.Dispose();
        }
    }
}
