using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft.Core.Database
{
    public class SettingsRepository
    {
        private string Data = $"Data Source={Path.Combine(AppContext.BaseDirectory, "Data-test.db")}";

        public async Task<Setting> GetAllSettings()
        {
            Setting settings = new Setting();

            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT * FROM Settings", connection);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            settings = new Setting(
                                Convert.ToInt32(reader["Id"]),
                                reader["BotToken"]?.ToString(),
                                Convert.ToInt32(reader["AutoBot"]),
                                Convert.ToInt32(reader["TrayOnStart"]),
                                Convert.ToInt32(reader["RunAtStartup"]),
                                Convert.ToInt32(reader["AutoReconnect"]),
                                reader["Notifications"] == DBNull.Value ? null : Convert.ToInt32(reader["Notifications"]),
                                reader["ProxyHost"]?.ToString(),
                                reader["ProxyPort"]?.ToString(),
                                reader["ProxyUsername"]?.ToString(),
                                reader["ProxyPassword"]?.ToString()
                            );
                        }
                    }
                }
                return settings;
            }
            catch (SqliteException ex) { return new Setting(); }
        }

        public async Task<Setting> GetTokenAndProxySettings()
        {
            Setting settings = new Setting();

            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT BotToken, ProxyHost, ProxyPort, ProxyUsername, ProxyPassword FROM Settings", connection);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            settings = new Setting(
                                reader["BotToken"]?.ToString(),
                                reader["ProxyHost"]?.ToString(),
                                reader["ProxyPort"]?.ToString(),
                                reader["ProxyUsername"]?.ToString(),
                                reader["ProxyPassword"]?.ToString()
                            );
                        }
                    }
                }
                return settings;
            }
            catch (SqliteException ex) { return new Setting(); }
        }

        public async Task SaveSettings(Setting? settings)
        {
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand updateCommand = new SqliteCommand("UPDATE Settings SET " +
                        "(BotToken, AutoBot, AutoReconnect, ProxyHost, ProxyPort, ProxyUsername, ProxyPassword, TrayOnStart, RunAtStartup, Notifications)" +
                        " = (@BotToken, @AutoBot, @AutoReconnect, @ProxyHost, @ProxyPort, @ProxyUsername, @ProxyPassword, @TrayOnStart, @RunAtStartup, @Notifications) " +
                        "WHERE ID == 1;", connection);
                    updateCommand.Parameters.AddWithValue("@BotToken", settings?.BotToken);
                    updateCommand.Parameters.AddWithValue("@AutoBot", settings?.AutoBot);
                    updateCommand.Parameters.AddWithValue("@AutoReconnect", settings?.AutoReconnect);
                    updateCommand.Parameters.AddWithValue("@ProxyHost", settings?.ProxyHost);
                    updateCommand.Parameters.AddWithValue("@ProxyPort", settings?.ProxyPort);
                    updateCommand.Parameters.AddWithValue("@ProxyUsername", settings?.ProxyUsername);
                    updateCommand.Parameters.AddWithValue("@ProxyPassword", settings?.ProxyPassword);
                    updateCommand.Parameters.AddWithValue("@TrayOnStart", settings?.TrayOnStart);
                    updateCommand.Parameters.AddWithValue("@RunAtStartup", settings?.RunAtStartup);
                    updateCommand.Parameters.AddWithValue("@Notifications", settings?.Notifications);

                    await updateCommand.ExecuteNonQueryAsync();
                }
            }
            catch { }
        }
    }
}
