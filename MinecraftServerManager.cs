using CoreRCON;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Net;

namespace TelegramBotMinecraft
{
    public class MinecraftServerManager
    {
        private static MinecraftServerManager _currentInstance;

        public static UserControl_Console userControl_Console;

        private Process? process = null;
        private int processId = -1;

        private Dictionary<string, string> ServerData = new();
        private Dictionary<string, Process> serverProcesses = new();

        public static async Task<bool> Start(string ServerName)
        {
            _currentInstance = new MinecraftServerManager();
            return await _currentInstance.StartServer(ServerName);
        }

        public static async Task<bool> Stop(string ServerName)
        {
            if (_currentInstance == null)
            {
                _currentInstance = new MinecraftServerManager();
            }
            
            return await _currentInstance.StopServer(ServerName);
        }

        private async Task<bool> StartServer(string ServerName)
        {
            await GetServerData(ServerName);

            if (ServerData["ID_Process"] == "-1")
            {
                try
                {
                    process = new Process();
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\Program Files\Java\jdk-21.0.11\bin\java.exe",
                        WorkingDirectory = ServerData["Path_Server"],
                        Arguments = ServerData["Java_args"],
                        CreateNoWindow = true,
                        RedirectStandardInput = true,
                        UseShellExecute = false
                    };
                    process.Start();
                    processId = process.Id;
                    serverProcesses[ServerName] = process;

                    using (var connection = new SqliteConnection("Data Source=Data.db"))
                    {
                        await connection.OpenAsync();
                        SqliteCommand command = new SqliteCommand("UPDATE Servers SET ID_Process = @ProcessId WHERE Name == @ServerName", connection);
                        command.Parameters.AddWithValue("@ProcessId", processId);
                        command.Parameters.AddWithValue("@ServerName", ServerName);
                        await command.ExecuteNonQueryAsync();
                    }
                    userControl_Console.TriggerServerInfoRefresh(ServerName, 1);
                    return true;
                }
                catch (Exception ex) { MessageBox.Show($"Ошибка при запуске: {ex.Message}"); return false; }
            }
            return false;
        }

        public async Task<bool> StopServer(string ServerName)
        {
            await GetServerData(ServerName);
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

                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("UPDATE Servers SET ID_Process = @ProcessId WHERE Name == @ServerName", connection);
                    command.Parameters.AddWithValue("@ProcessId", -1);
                    command.Parameters.AddWithValue("@ServerName", ServerName);
                    await command.ExecuteNonQueryAsync();
                }
                userControl_Console.TriggerServerInfoRefresh(ServerName, 0);
                return true;
            }
            catch { return false; }
        }

        public async Task SendCommand(string serverName, string command)
        {
            await GetServerData(serverName);

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

        private async Task GetServerData(string ServerName)
        {
            ServerData.Clear();
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                await connection.OpenAsync();
                SqliteCommand command = new SqliteCommand("SELECT Path_Server, ID_Process, Java_args, Rcon_Enable, Rcon_Port, Rcon_Pass FROM Servers WHERE Name == @ServerName", connection);
                command.Parameters.AddWithValue("@ServerName", ServerName);
                SqliteDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ServerData["Path_Server"] = reader["Path_Server"].ToString();
                    ServerData["Java_args"] = reader["Java_args"].ToString();
                    ServerData["ID_Process"] = reader["ID_Process"].ToString();
                    ServerData["Rcon_Enable"] = reader["Rcon_Enable"].ToString();
                    ServerData["Rcon_Port"] = reader["Rcon_Port"].ToString();
                    ServerData["Rcon_Pass"] = reader["Rcon_Pass"].ToString();
                }
            }
        }
    }
}
