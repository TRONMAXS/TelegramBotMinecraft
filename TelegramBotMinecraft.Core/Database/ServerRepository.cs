using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
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
                    SqliteCommand command = new SqliteCommand("SELECT * FROM Servers", connection);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AllServersList.Add(new Server(
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
                return AllServersList;
            }
            catch (SqliteException ex) { return new List<Server>(); }
        }

        public async Task<List<Server>> GetAllServersIdAndName()
        {
            List<Server> ServersList = new List<Server>();
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
                            ServersList.Add(new Server(Convert.ToInt32(reader["ID"]), reader["Name"].ToString()));
                        }
                    }
                }
                return ServersList;
            }
            catch (SqliteException ex) { return new List<Server>(); }
        }

        public async Task<Server> GetServerByName(string Name)
        {
            Server server = new Server();
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
                            server = new Server(
                            Convert.ToInt32(reader["ID"]),
                            reader["Name"].ToString() ?? string.Empty,
                            reader["Connected"] as string,
                            reader["Path_Server"] as string,
                            reader["ID_Process"] == DBNull.Value ? -1 : Convert.ToInt32(reader["ID_Process"]),
                            reader["Java_args"] as string,
                            reader["Rcon_Enable"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Rcon_Enable"]),
                            reader["Rcon_Port"] == DBNull.Value ? null : Convert.ToInt32(reader["Rcon_Port"]),
                            reader["Rcon_Pass"] == DBNull.Value ? null : reader["Rcon_Pass"].ToString()
                            );
                        }
                    } 
                }
                return server;
            }
            catch (SqliteException ex) { return new Server(); }
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

        public async Task AddServer(Server server)
        {
            try
            {
                string sqlAddServer = "INSERT INTO Servers (Name, Connected, Path_Server, Java_args, Rcon_Enable, Rcon_Port, Rcon_Pass) " +
                          "VALUES (@Name, @Connected, @Path_Server, @Java_args, @Rcon_Enable, @Rcon_Port, @Rcon_Pass)";

                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    using (SqliteCommand command = new SqliteCommand(sqlAddServer, connection))
                    {
                        command.Parameters.AddWithValue("@ServerID", server.Id);
                        command.Parameters.AddWithValue("@Name", server.Name);

                        command.Parameters.AddWithValue("@Connected", server.Connected ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Path_Server", server.PathServer ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Java_args", server.JavaArgs ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Rcon_Enable", server.RconEnable);
                        command.Parameters.AddWithValue("@Rcon_Port", server.RconPort.HasValue ? (object)server.RconPort.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@Rcon_Pass", server.RconPass ?? (object)DBNull.Value);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch { }
        }

        public async Task UpdateServer(string ServerName, int PID)
        {
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("UPDATE Servers SET ID_Process = @ProcessId WHERE Name = @ServerName", connection);
                    command.Parameters.AddWithValue("@ProcessId", PID);
                    command.Parameters.AddWithValue("@ServerName", ServerName);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch { }
        }

        public async Task UpdateServer(Server server)
        {
            try
            {
                string sqlAddServer = "UPDATE Servers SET Name = @Name, Connected = @Connected, " +
                    "Path_Server = @Path_Server, Java_args = @Java_args, " +
                    "Rcon_Enable = @Rcon_Enable, Rcon_Port = @Rcon_Port, " +
                    "Rcon_Pass = @Rcon_Pass " +
                    "WHERE ID = @ServerID";

                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    using (SqliteCommand command = new SqliteCommand(sqlAddServer, connection))
                    {
                        command.Parameters.AddWithValue("@ServerID", server.Id);
                        command.Parameters.AddWithValue("@Name", server.Name);

                        command.Parameters.AddWithValue("@Connected", server.Connected ?? (object)server.Connected ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Path_Server", server.PathServer ?? (object)server.PathServer ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Java_args", server.JavaArgs ?? (object)server.JavaArgs ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Rcon_Enable", server.RconEnable);
                        command.Parameters.AddWithValue("@Rcon_Port", server.RconPort.HasValue ? (object)server.RconPort.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@Rcon_Pass", server.RconPass ?? (object)server.RconPass ?? DBNull.Value);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch { }
        }

        public async Task DeleteServer(int Id)
        {
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    using (SqliteCommand command = new SqliteCommand("DELETE FROM Servers WHERE ID = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch { }
        }

        public async Task SaveUserServers()
        {
            
        }
    }
}
