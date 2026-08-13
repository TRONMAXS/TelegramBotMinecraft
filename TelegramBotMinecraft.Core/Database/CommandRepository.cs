using Microsoft.Data.Sqlite;
using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft.Core.Database
{
    public class CommandRepository
    {
        private string Data = $"Data Source={Path.Combine(AppContext.BaseDirectory, "Data-test.db")}";

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
                            AllCommandsList.Add(new Command(Convert.ToInt32(reader["ID"]), reader["Command"].ToString()));
                        }
                    }
                }
                return AllCommandsList;
            }
            catch (SqliteException ex) { return new List<Command>(); }
        }

        public async Task<List<Command>> GetCommandsByUserIdAsync(long userId)
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

        public async Task<bool> HasAccessToCommandAsync(long userId, int commandId)
        {
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand(@" SELECT EXISTS (
                                                                      SELECT 1 
                                                                      FROM UserCommands 
                                                                      WHERE ID_User = @userId AND ID_Command = @commandId);", connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@commandId", commandId);


                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (Convert.ToUInt32(reader[0]) == 0)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (SqliteException ex) { return false; }
        }

        public async Task SaveUserCommandsAsync(int userId, List<int> commandsId)
        {
            using (var connection = new SqliteConnection(Data))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        string deleteSql = "DELETE FROM UserCommands WHERE ID_User = @UserID;";
                        using (var delCommand = new SqliteCommand(deleteSql, connection, transaction as SqliteTransaction))
                        {
                            delCommand.Parameters.AddWithValue("@UserID", userId);
                            await delCommand.ExecuteNonQueryAsync();
                        }

                        if (commandsId != null && commandsId.Count > 0)
                        {
                            string insertSql = "INSERT INTO UserCommands (ID_User, ID_Command) VALUES (@UserID, @CommandID);";
                            using (var insCommand = new SqliteCommand(insertSql, connection, transaction as SqliteTransaction))
                            {
                                var userParam = insCommand.Parameters.Add("@UserID", SqliteType.Integer);
                                var commandParam = insCommand.Parameters.Add("@CommandID", SqliteType.Integer);

                                userParam.Value = userId;

                                foreach (int Id in commandsId)
                                {
                                    commandParam.Value = Id;
                                    await insCommand.ExecuteNonQueryAsync();
                                }
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
