using Microsoft.Data.Sqlite;
using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft.Core.Database
{
    public class UserRepository
    {

        private string Data = $"Data Source={Path.Combine(AppContext.BaseDirectory, "Data-test.db")}";

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

        public async Task AddUser(string? name, int? id)
        {
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("INSERT INTO Users (Name, ID_TG) VALUES (@UserName, @UserId);", connection);
                    command.Parameters.AddWithValue("@UserName", name);
                    command.Parameters.AddWithValue("@UserId", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch { }
        }

        public async Task UpdateUser(string? name, int? newId, int? oldId)
        {
            string sqlUpdateUser = "UPDATE Users SET Name = @UserName, ID_TG = @UserNewId WHERE ID_TG = @UserOldId;";
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand(sqlUpdateUser, connection);
                    command.Parameters.AddWithValue("@UserName", name);
                    command.Parameters.AddWithValue("@UserNewId", newId);
                    command.Parameters.AddWithValue("@UserOldId", oldId);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch { }
        }

        public async Task DeleteUser(int? id)
        {
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("DELETE FROM Users WHERE (ID_TG) = @UserId;", connection);
                    command.Parameters.AddWithValue("@UserId", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch { }
        }

    }
}
