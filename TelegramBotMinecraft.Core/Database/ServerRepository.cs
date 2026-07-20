using Microsoft.Data.Sqlite;
using System.Diagnostics;
using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft.Core.Database
{
    public class ServerRepository
    {
        private string Data = "Data Source=Data-test.db";


        public ServerRepository() { }


        public async Task<List<Server>> GetAllServers()
        {
            List<Server> AllServersList = new List<Server>();
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT ID, Name FROM Servers", connection);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AllServersList.Add(new Server(Convert.ToInt32(reader["ID"]), reader["Name"].ToString()));
                        }
                    }
                }
                return AllServersList;
            }
            catch (SqliteException ex) { return new List<Server>(); }
        }

        public async Task<List<Server>> GetServerByName(string Name)
        {
            var server = new List<Server>();
            try
            {

                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT * FROM Servers WHERE Name = @ServerName", connection);
                    command.Parameters.AddWithValue("@ServerName", Name);
                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            server.Add(new Server(
                            Convert.ToInt32(reader["ID"]),
                            reader["Name"].ToString() ?? string.Empty,
                            reader["Connected"] as string,
                            reader["Path_Server"] as string,
                            reader["ID_Process"] == DBNull.Value ? -1 : Convert.ToInt32(reader["ID_Process"]),
                            reader["Java_args"] as string,
                            reader["Rcon_Enable"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Rcon_Enable"]),
                            reader["Rcon_Port"] == DBNull.Value ? null : Convert.ToInt32(reader["Rcon_Port"]),
                            reader["Rcon_Pass"] == DBNull.Value ? null : reader["Rcon_Pass"].ToString()
                            ));
                        }
                    }

                }
            }
            catch (SqliteException ex) { return new List<Server>(); }
            return server;
        }

        public async Task<List<Server>> GetServersByUserIdAsync(int userId)
        {
            List<Server> AllServersList = new List<Server>();

            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand(@" SELECT s.ID, s.Name 
                                                                 FROM UserServers us
                                                                 JOIN Servers s ON us.ID_Server = s.ID
                                                                 WHERE us.ID_User = @userId", connection);
                    command.Parameters.AddWithValue("@userId", userId);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AllServersList.Add(new Server(Convert.ToInt32(reader["ID"]), reader["Name"].ToString()));
                        }
                    }
                }
                return AllServersList;
            }
            catch (SqliteException ex) { return new List<Server>(); }
        }

        public async Task AddServer()
        {

        }

        public async Task UpdateServer(string ServerName, int PID)
        {
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("UPDATE Servers SET ID_Process = @ProcessId WHERE Name == @ServerName", connection);
                    command.Parameters.AddWithValue("@ProcessId", PID);
                    command.Parameters.AddWithValue("@ServerName", ServerName);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch { }
        }
        public async Task UpdateServer(List<Server> ServerData)
        {
            try
            {
                
            }
            catch { }
        }

        public async Task DeleteServer()
        {

        }

        public async Task SaveUserServers()
        {

        }
    }
}
