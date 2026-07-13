using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotMinecraft.Core.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TelegramBotMinecraft.Core.Database
{
    public class ServerRepository
    {
        private string Data = "Data Source=Data-test.db";


        public ServerRepository() { }


        public async Task<List<string>> GetAllServersNames()
        {
            List<string> AllServersList = new List<string>();
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT Name FROM Servers", connection);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AllServersList.Add(reader[0].ToString());
                        }
                    }
                }
                return AllServersList;
            }
            catch (SqliteException ex) { return null; }
        }

        public async Task GetServerByName(string Name)
        {
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT * FROM Servers WHERE Name = @ServerName", connection);
                    command.Parameters.AddWithValue("@ServerName", Name);
                    var result = await command.ExecuteScalarAsync();
                    if (result == null) return;
                    //pathToLogsServer = Path.Combine(result.ToString(), "logs", "latest.log");
                }
            }
            catch (SqliteException ex) { return; }
        }

        public async Task AddServer()
        {

        }

        public async Task UpdateServer()
        {

        }

        public async Task DeleteServer()
        {

        }

        public async Task SaveUserServers()
        {

        }
    }
}
