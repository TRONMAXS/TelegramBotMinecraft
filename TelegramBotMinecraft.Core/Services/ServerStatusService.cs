using System.Diagnostics;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;
using static TelegramBotMinecraft.Core.Models.ServerStatusModel;

namespace TelegramBotMinecraft.Core.Services
{
    public class ServerStatusService
    {

        public async Task<ServerStatus> GetStatus(string ServerName)
        {
            ServerRepository repository = new ServerRepository();
            var serverData = await repository.GetServerByName(ServerName);
            if (serverData == null || serverData.Count == 0) return ServerStatus.Offline;
            var Data = serverData[0];

            var logTask = CheckingLog(Data.PathServer?.ToString());
            var processTask = CheckingProcess(Data.IdProcess);
            await Task.WhenAll(logTask, processTask);

            return await StatusDetermination(logTask.Result, processTask.Result);
        }

        private async Task<ServerStatus> CheckingLog(string? PathServer)
        {
            string logFilePath = Path.Combine(PathServer ?? string.Empty, "logs", "latest.log");
            if (!File.Exists(logFilePath)) return ServerStatus.Offline;

            Regex doneRegex = new Regex(@"^\[.*?\] \[Server thread\/INFO\]: Done \(.*?\)!");
            Regex doneRegex2 = new Regex(@"^\[.*?\] \[.*?\] \[.*?\]: Done \(.*?\)!");


            try
            {
                using (var fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(fs, System.Text.Encoding.UTF8, true, 1024, leaveOpen: true))
                    {
                        string line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            if (doneRegex.IsMatch(line) || doneRegex2.IsMatch(line))
                            {
                                return ServerStatus.Online;
                            }
                        }
                    }
                }
                return ServerStatus.Offline;
            }
            catch { return ServerStatus.Offline; }
        }        

        private async Task<ServerStatus> CheckingProcess(int? PID)
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
        private async Task<ServerStatus> StatusDetermination(ServerStatus statusLog, ServerStatus statusProcess)
        {
            if (statusProcess == ServerStatus.Online && statusLog == ServerStatus.Online)
            {
                return ServerStatus.Online;
            }

            if (statusProcess == ServerStatus.Online && statusLog == ServerStatus.Offline)
            {
                return ServerStatus.Starting;
            }

            return ServerStatus.Offline;
        }
    }
}
