using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotMinecraft.Core.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TelegramBotMinecraft.Core.Database
{
    public class UserRepository
    {

        private string Data = "Data Source=Data-test.db";

        public async Task<List<User>> GetAllUserNamesAndId()
        {
            var users = new List<User>();

            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT * FROM Users", connection);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User(reader["Name"].ToString(), Convert.ToInt32(reader["ID_TG"])));
                        }
                    }
                }
                return users;
            }
            catch (SqliteException ex) { return new List<User>(); }
        }

        public async Task GetUserByIdTg(string Id)
        {
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT * FROM Users WHERE ID = @UserID", connection);
                    command.Parameters.AddWithValue("@UserID", Id);
                    var result = await command.ExecuteScalarAsync();
                    if (result == null) return;
                }
            }
            catch (SqliteException ex) { return; }
        }

        public async Task AddUser(string UserName, string ID_TG)
        {

        }

        public async Task UpdateUser(string UserName, string ID_TG, string UserID)
        {
            string sqlUpdateUser = "UPDATE Users SET Name = @UserName, ID_TG = @UserIDTG WHERE ID = @UserID;";
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand(sqlUpdateUser, connection);
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@UserIDTG", ID_TG);
                    command.Parameters.AddWithValue("@UserID", UserID);
                    await command.ExecuteNonQueryAsync();
                }

                LoggerService.MessageAppInfo($"Пользователь [{UserName}] обновлен");
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при обновление пользователя [{UserName}] : {ex.Message}");
            }

        }

        public async Task DeleteUser(string IdTg)
        {

        }

    }
}
