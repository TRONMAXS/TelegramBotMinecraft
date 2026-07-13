using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft.Core.Database
{
    internal class SettingsRepository
    {
        private string Data = "Data-test.db";
        List<Setting> settingList = new List<Setting>();


        public async Task GetAllSettings()
        {
            using (var connection = new SqliteConnection(Data))
            {
                await connection.OpenAsync();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Settings", connection);
                SqliteDataReader reader = await command.ExecuteReaderAsync();


                while (await reader.ReadAsync())
                {
                    /*tb_BotToken.Text = reader["BotToken"].ToString();
                    chk_AutoStartBot.Checked = Convert.ToInt32(reader["Auto_Bot"]) == 1 ? true : false;
                    chk_StartToTray.Checked = Convert.ToInt32(reader["TrayOnStart"]) == 1 ? true : false;
                    chk_AutoStartup.Checked = Convert.ToInt32(reader["RunAtStartup"]) == 1 ? true : false;
                    chk_AutoRestartBot.Checked = Convert.ToInt32(reader["AutoReconnect"]) == 1 ? true : false;
                    chk_PushNotifications.Checked = Convert.ToInt32(reader["AutoReconnect"]) == 1 ? true : false;
                    tb_ProxyHost.Text = reader["Proxy_Host"].ToString();
                    tb_ProxyPort.Text = reader["Proxy_Port"].ToString();
                    tb_ProxyUsername.Text = reader["Proxy_Username"].ToString();
                    tb_ProxyPassword.Text = reader["Proxy_Password"].ToString();*/
                }
            }
        }

        public async Task SaveSettings()
        {
            try
            {
                using (var connection = new SqliteConnection(Data))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand("UPDATE Settings SET (TrayOnStart, RunAtStartup, Notifications) = (@TrayOnStart, @RunAtStartup, @Notifications) WHERE ID == 1;", connection);
                    /*command.Parameters.AddWithValue("@TrayOnStart", Convert.ToInt32(chk_StartToTray.Checked));
                    command.Parameters.AddWithValue("@RunAtStartup", Convert.ToInt32(chk_AutoStartup.Checked));
                    command.Parameters.AddWithValue("@Notifications", Convert.ToInt32(chk_PushNotifications.Checked));*/
                    command.ExecuteNonQuery();
                }
                LoggerService.MessageAppInfo("Настройки приложения сохранены.");
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при сохранении настроек приложения: {ex.Message}");
            }
        }

        public async Task SaveSettingsBOT()
        {
            try
            {

                string OldBotToken = "";
                using (var connection = new SqliteConnection(Data))
                {
                    await connection.OpenAsync();
                    SqliteCommand getOldTokenCmd = new SqliteCommand("SELECT BotToken FROM Settings", connection);
                    OldBotToken = getOldTokenCmd.ExecuteScalar().ToString();

                    SqliteCommand updateCommand = new SqliteCommand("UPDATE Settings SET (BotToken, Auto_Bot, AutoReconnect, Proxy_Host, Proxy_Port, Proxy_Username, Proxy_Password)" +
                        " = (@BotToken, @Auto_Bot, @AutoReconnect, @Proxy_Host, @Proxy_Port, @Proxy_Username, @Proxy_Password) WHERE ID == 1;", connection);
                    /*updateCommand.Parameters.AddWithValue("@BotToken", tb_BotToken.Text.Trim());
                    updateCommand.Parameters.AddWithValue("@Auto_Bot", Convert.ToInt32(chk_AutoStartBot.Checked));
                    updateCommand.Parameters.AddWithValue("@AutoReconnect", Convert.ToInt32(chk_AutoRestartBot.Checked));
                    updateCommand.Parameters.AddWithValue("@Proxy_Host", tb_ProxyHost.Text.Trim());
                    updateCommand.Parameters.AddWithValue("@Proxy_Port", tb_ProxyPort.Text.Trim());
                    updateCommand.Parameters.AddWithValue("@Proxy_Username", tb_ProxyUsername.Text.Trim());
                    updateCommand.Parameters.AddWithValue("@Proxy_Password", tb_ProxyPassword.Text.Trim());*/
                    await updateCommand.ExecuteNonQueryAsync();

                }

                LoggerService.MessageAppInfo("Настройки бота обновлены.");
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при сохранении настроек бота: {ex.Message}");
            }
        }
    }
}
