using CoreRCON;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Net;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft
{
    public class MinecraftServerManager
    {
        private Process? process = null;
        private int processId = -1;

        private Dictionary<string, Process> serverProcesses = new();

        public async Task<bool> StartServer(string ServerName)
        {
            var ServerData = await GetServerData(ServerName);

            if (ServerData.IdProcess == -1)
            {
                try
                {
                    process = new Process();
                    process?.StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Program Files\Java\jdk-25.0.3\bin\javaw.exe",
                        WorkingDirectory = ServerData.PathServer,
                        Arguments = ServerData.JavaArgs,
                        CreateNoWindow = true,
                        RedirectStandardInput = true,
                        UseShellExecute = false
                    };
                    process?.Start();
                    processId = process.Id;
                    serverProcesses[ServerName] = process;

                    ServerRepository repository = new ServerRepository();
                    await repository.UpdateServer(ServerName, processId);

                    return true;
                }
                catch (Exception ex) { /*MessageBox.Show($"Ошибка при запуске: {ex.Message}");*/ return false; }
            }
            return false;
        }

        public async Task<bool> StopServer(string ServerName)
        {
            var ServerData = await GetServerData(ServerName);
            try
            {
                if (serverProcesses.TryGetValue(ServerName, out var proc) && !proc.HasExited)
                {
                    await proc.StandardInput.WriteLineAsync("stop");
                    await proc.StandardInput.FlushAsync();
                }
                else
                {
                    using (var rcon = new RCON(IPAddress.Parse("127.0.0.1"), Convert.ToUInt16(ServerData.RconPort), ServerData.RconPass))
                    {
                        await rcon.SendCommandAsync("stop");
                    }
                }

                ServerRepository repository = new ServerRepository();
                await repository.UpdateServer(ServerName, -1);

                return true;
            }
            catch { return false; }
        }

        public async Task SendCommand(string serverName, string command)
        {
            var ServerData = await GetServerData(serverName);

            if (serverProcesses.TryGetValue(serverName, out var proc) && !proc.HasExited)
            {
                await proc.StandardInput.WriteLineAsync(command);
                await proc.StandardInput.FlushAsync();
            }
            else
            {
                using (var rcon = new RCON(IPAddress.Parse("127.0.0.1"), Convert.ToUInt16(ServerData.RconPort), ServerData.RconPass))
                {
                    await rcon.SendCommandAsync(command);
                }
            }
        }

        public async Task<Server> GetServerData(string ServerName)
        {
            Server Server = new();

            ServerRepository repository = new ServerRepository();
            var serverData = await repository.GetServerByName(ServerName);
            if (serverData == null) return new Server();

            Server.PathServer = serverData.PathServer ?? string.Empty;
            Server.JavaArgs = serverData.JavaArgs ?? string.Empty;
            Server.IdProcess = serverData.IdProcess;
            Server.RconEnable = serverData.RconEnable;
            Server.RconPort = serverData.RconPort;
            Server.RconPass = serverData.RconPass ?? string.Empty;

            return Server;
        }
    }
}
