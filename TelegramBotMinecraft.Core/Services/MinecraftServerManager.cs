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
        private MinecraftServerManager? _currentInstance;

        private Process? process = null;
        private int processId = -1;

        private Dictionary<string, Process> serverProcesses = new();

        public async Task<bool> StartServer(string ServerName)
        {
            var ServerData = await GetServerData(ServerName);

            if (ServerData["ID_Process"] == "-1")
            {
                try
                {
                    process = new Process();
                    process?.StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Program Files\Java\jdk-25.0.3\bin\javaw.exe",
                        WorkingDirectory = ServerData["Path_Server"],
                        Arguments = ServerData["Java_args"],
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
                    using (var rcon = new RCON(IPAddress.Parse("127.0.0.1"), Convert.ToUInt16(ServerData["Rcon_Port"]), ServerData["Rcon_Pass"]))
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
                using (var rcon = new RCON(IPAddress.Parse("127.0.0.1"), Convert.ToUInt16(ServerData["Rcon_Port"]), ServerData["Rcon_Pass"]))
                {
                    await rcon.SendCommandAsync(command);
                }
            }
        }

        public async Task<Dictionary<string, string>> GetServerData(string ServerName)
        {
            Dictionary<string, string> ServerData = new();

            ServerRepository repository = new ServerRepository();
            var serverData = await repository.GetServerByName(ServerName);
            if (serverData == null || serverData.Count == 0) return new Dictionary<string, string>();

            var Data = serverData[0];

            ServerData["Path_Server"] = Data.PathServer ?? string.Empty;
            ServerData["Java_args"] = Data.JavaArgs ?? string.Empty;
            ServerData["ID_Process"] = Data.IdProcess.ToString() ?? string.Empty;
            ServerData["Rcon_Enable"] = Data.RconEnable.ToString() ?? string.Empty;
            ServerData["Rcon_Port"] = Data.RconPort?.ToString() ?? string.Empty;
            ServerData["Rcon_Pass"] = Data.RconPass?.ToString() ?? string.Empty;

            return ServerData;
        }

        /*private async Task StopServer(string ServerName)
        {
            await GetServerData(ServerName);

            if (ServerData[2] != "-1")
            {
                try
                {
                    process = Process.GetProcessById(Convert.ToInt32(ServerData[2]));

                    await process.StandardInput.WriteLineAsync("stop");
                    if (!process.WaitForExit(30000)) MessageBox.Show("Сервер не выключился.");
                    else MessageBox.Show("Сервер остановлен.");

                    processId = -1;
                    process.Dispose();
                    process = null;

                    using (var connection = new SqliteConnection("Data Source=Data.db"))
                    {
                        await connection.OpenAsync();
                        SqliteCommand command = new SqliteCommand("UPDATE Servers SET ID_Process = @ProcessId WHERE Name == @ServerName", connection);
                        command.Parameters.AddWithValue("@ProcessId", processId);
                        command.Parameters.AddWithValue("@ServerName", ServerName);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex) { MessageBox.Show($"Ошибка при остановке: {ex.Message}"); }
            }
            else MessageBox.Show("Остановка невозможна: сервер не запущен.");

        }*/

    }
}
