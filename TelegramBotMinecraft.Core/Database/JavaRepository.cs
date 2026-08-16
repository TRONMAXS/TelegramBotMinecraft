using Microsoft.Data.Sqlite;
using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft.Core.Database
{
    public class JavaRepository
    {

        private readonly string _connectionString;

        public JavaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<JavaManager>> GetAllJava()
        {
            var java = new List<JavaManager>();

            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT * FROM Java", connection);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            java.Add(new JavaManager(reader["Name"].ToString(), reader["Version"].ToString(), reader["Architecture"].ToString(), reader["Path"].ToString()));
                        }
                    }
                }
                return java;
            }
            catch (SqliteException ex) { return java; }
        }

        public async Task<JavaManager> GetJavaByName(string Name)
        {
            var java = new JavaManager();

            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT * FROM Java WHERE Name = @Name", connection);
                    command.Parameters.AddWithValue("@Name", Name);
                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            java = new JavaManager(reader["Name"].ToString(), reader["Version"].ToString(), reader["Architecture"].ToString(), reader["Path"].ToString());
                        }
                    }
                }
                return java;
            }
            catch (SqliteException ex) { return java; }
        }

        public async Task Add(List<JavaManager> javaList)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        string insertSql = "INSERT INTO Java (Name, Version, Architecture, Path) VALUES (@Name, @Version, @Architecture, @Path);";

                        using (var insCommand = new SqliteCommand(insertSql, connection, transaction as SqliteTransaction))
                        {
                            var nameParam = insCommand.Parameters.Add("@Name", SqliteType.Text);
                            var versionParam = insCommand.Parameters.Add("@Version", SqliteType.Text);
                            var architectureParam = insCommand.Parameters.Add("@Architecture", SqliteType.Text);
                            var pathParam = insCommand.Parameters.Add("@Path", SqliteType.Text);

                            foreach (var java in javaList)
                            {
                                nameParam.Value = java.Name;
                                versionParam.Value = java.Version;
                                architectureParam.Value = java.Architecture;
                                pathParam.Value = java.Path;
                                await insCommand.ExecuteNonQueryAsync();
                            }
                        }

                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task Update(JavaManager javaManager, string? oldName)
        {
            string sqlUpdateUser = "UPDATE Java SET Name = @Name, Version = @Version, Architecture = @Architecture, Path = @Path WHERE Name = @OldName;";
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand(sqlUpdateUser, connection);
                    command.Parameters.AddWithValue("@Name", javaManager.Name);
                    command.Parameters.AddWithValue("@Version", javaManager.Version);
                    command.Parameters.AddWithValue("@Architecture", javaManager.Architecture);
                    command.Parameters.AddWithValue("@Path", javaManager.Path);
                    command.Parameters.AddWithValue("@OldName", oldName);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch { }
        }

        public async Task Delete(string? Name)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("DELETE FROM Java WHERE (Name) = @Name;", connection);
                    command.Parameters.AddWithValue("@Name", Name);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch { }
        }

        public async Task Delete(List<JavaManager> javaList)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        string insertSql = "DELETE FROM Java WHERE (Name) = @Name;";

                        using (var insCommand = new SqliteCommand(insertSql, connection, transaction as SqliteTransaction))
                        {
                            var nameParam = insCommand.Parameters.Add("@Name", SqliteType.Text);

                            foreach (var java in javaList)
                            {
                                nameParam.Value = java.Name;
                                await insCommand.ExecuteNonQueryAsync();
                            }
                        }

                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

    }
}
