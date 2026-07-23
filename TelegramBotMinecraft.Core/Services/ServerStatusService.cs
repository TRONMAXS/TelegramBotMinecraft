using System.Diagnostics;
using System.Text.RegularExpressions;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;
using static TelegramBotMinecraft.Core.Models.ServerStatusModel;

namespace TelegramBotMinecraft.Core.Services
{
    public class ServerStatusService
    {
        private readonly ServerRepository _repository;

        private readonly Regex startingRegex = new Regex(@"Starting minecraft server", RegexOptions.Compiled);
        private readonly Regex doneRegex = new Regex(@"Done \(.*?\)!", RegexOptions.Compiled);
        private readonly Regex stoppingRegex = new Regex(@"Stopping the server", RegexOptions.Compiled);
        private readonly Regex warningRegex = new Regex(@"Can't keep up!", RegexOptions.Compiled);

        public ServerStatusService(ServerRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ServerStatusInfo>> CheckAllServers()
        {
            List<ServerStatusInfo> serverStatusList = new List<ServerStatusInfo>();

            var servers = await _repository.GetAllServers();
            if (servers == null) return serverStatusList;

            foreach (var server in servers)
            {
                var serverInfo = await CheckServer(server);
                serverStatusList.Add(serverInfo);
            }

            return serverStatusList;
        }

        private async Task<ServerStatusInfo> CheckServer(Server server)
        {
            var status = DetermineStatus(await CheckLog(server.PathServer), CheckProcess(server.IdProcess));

            return new ServerStatusInfo(server.Name, status);
        }

        private async Task<ServerStatus> CheckLog(string? pathServer)
        {
            string logFilePath = Path.Combine(pathServer ?? string.Empty, "logs", "latest.log");

            if (!File.Exists(logFilePath)) return ServerStatus.Offline;

            try
            {
                var lines = new List<string>();

                using var fs = new FileStream(
                    logFilePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite);

                using var reader = new StreamReader(fs);

                string? line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }

                foreach (string currentLine in lines.AsEnumerable().Reverse())
                {
                    if (stoppingRegex.IsMatch(currentLine))
                    {
                        return ServerStatus.Offline;
                    }

                    if (warningRegex.IsMatch(currentLine))
                    {
                        return ServerStatus.Warning;
                    }

                    if (doneRegex.IsMatch(currentLine))
                    {
                        return ServerStatus.Online;
                    }

                    if (startingRegex.IsMatch(currentLine))
                    {
                        return ServerStatus.Starting;
                    }
                }
                return ServerStatus.Offline;
            }
            catch
            {
                return ServerStatus.Offline;
            }
        }

        private ServerStatus CheckProcess(int? PID)
        {
            if (PID == null || PID <= 0) return ServerStatus.Offline;

            try
            {
                Process? process = Process.GetProcessById(PID.Value);
                if (!process.HasExited)
                {
                    string procName = process.ProcessName.ToLower();

                    if (procName == "javaw" || procName == "java")
                    {
                        return ServerStatus.Online;
                    }
                }

                return ServerStatus.Offline;
            }
            catch (ArgumentException) { return ServerStatus.Offline; }
            catch { return ServerStatus.Offline; }
        }

        private ServerStatus DetermineStatus(
        ServerStatus logStatus,
        ServerStatus processStatus)
        {

            if (processStatus == ServerStatus.Online && logStatus == ServerStatus.Online)
            {
                return ServerStatus.Online;
            }

            if (processStatus == ServerStatus.Offline && logStatus == ServerStatus.Online)
            {
                return ServerStatus.Offline;
            }

            if (processStatus == ServerStatus.Online && logStatus == ServerStatus.Warning)
            {
                return ServerStatus.Warning;
            }

            if (processStatus == ServerStatus.Online && logStatus == ServerStatus.Starting)
            {
                return ServerStatus.Starting;
            }

            return ServerStatus.Offline;
        }
    }
}
