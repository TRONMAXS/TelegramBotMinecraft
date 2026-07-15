using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotMinecraft.Core.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TelegramBotMinecraft.Core.Database
{
    public class CommandRepository
    {
        private string Data = "Data Source=Data-test.db";

        public async Task<List<Command>> GetAllCommandAsync()
        {
            List<Command> AllCommandsList = new List<Command>();

            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT * FROM Commands", connection);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AllCommandsList.Add(new Command(Convert.ToInt32(reader["ID"]), reader["ID"].ToString()));
                        }
                    }
                }
                return AllCommandsList;
            }
            catch (SqliteException ex) { return new List<Command>(); }
        }

        public async Task<List<Command>> GetCommandsByUserIdAsync(int userId)
        {
            List<Command> AllCommandsList = new List<Command>();

            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand(@" SELECT c.ID, c.Command 
                                                                 FROM UserCommands uc
                                                                 JOIN Commands c ON uc.ID_Command = c.ID
                                                                 WHERE uc.ID_User = @userId", connection);
                    command.Parameters.AddWithValue("@userId", userId);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AllCommandsList.Add(new Command(Convert.ToInt32(reader["ID"]), reader["Command"].ToString()));
                        }
                    }
                }
                return AllCommandsList;
            }
            catch (SqliteException ex) { return new List<Command>(); }
        }

        public async Task SaveUserCommandsAsync()
        {

        }

    }
}
