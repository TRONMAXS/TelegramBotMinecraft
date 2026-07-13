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

        public async Task<List<string>> GetAllCommand()
        {
            List<string> AllCommandsList = new List<string>();

            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT Command FROM Commands", connection);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AllCommandsList.Add(reader[0].ToString());
                        }
                    }
                }
                return AllCommandsList;
            }
            catch (SqliteException ex) { return null; }
        }

        public async Task SaveUserCommands()
        {

        }
    }
}
