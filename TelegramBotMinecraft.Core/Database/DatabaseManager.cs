using Microsoft.Data.Sqlite;
using System;

namespace TelegramBotMinecraft.Core.Database
{
    public class DatabaseManager
    {
        static string PathMain = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data.db");
        static string PathBackup = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataBuckup");

        public DatabaseManager()
        {

        }

        private static async Task CheckOrCreateBD()
        {
            try
            {
                using (var connection = new SqliteConnection($"Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    using (var enableForeignKeys = new SqliteCommand("PRAGMA foreign_keys = ON;", connection))
                    {
                        await enableForeignKeys.ExecuteNonQueryAsync();
                    }

                    string script = @"
            CREATE TABLE IF NOT EXISTS Commands (
                ID      INTEGER PRIMARY KEY NOT NULL UNIQUE,
                Command TEXT    UNIQUE NOT NULL
            );

            CREATE TABLE IF NOT EXISTS JavaVersions (
                ID INTEGER NOT NULL UNIQUE PRIMARY KEY
            );

            CREATE TABLE IF NOT EXISTS Servers (
                ID          INTEGER PRIMARY KEY NOT NULL UNIQUE,
                Name        TEXT    NOT NULL UNIQUE,
                Connected   TEXT,
                Path_Server TEXT,
                ID_Process  INTEGER DEFAULT (-1),
                Java_args   TEXT,
                Rcon_Enable INTEGER DEFAULT (0),
                Rcon_Port   INTEGER,
                Rcon_Pass   TEXT
            );

            CREATE TABLE IF NOT EXISTS Settings (
                ID             INTEGER PRIMARY KEY UNIQUE,
                BotToken       TEXT    NOT NULL,
                Auto_Bot       INTEGER NOT NULL DEFAULT (1),
                TrayOnStart    INTEGER NOT NULL DEFAULT (0),
                RunAtStartup   INTEGER NOT NULL DEFAULT (0),
                AutoReconnect  INTEGER NOT NULL DEFAULT (1),
                Notifications  INTEGER NOT NULL DEFAULT (1),
                Proxy_Host     TEXT,
                Proxy_Port     TEXT,
                Proxy_Username TEXT,
                Proxy_Password TEXT
            );

            CREATE TABLE IF NOT EXISTS Users (
                ID    INTEGER PRIMARY KEY UNIQUE,
                Name  TEXT    NOT NULL,
                ID_TG INTEGER UNIQUE ON CONFLICT ROLLBACK NOT NULL
            );

            CREATE TABLE IF NOT EXISTS UserCommands (
                ID_User    INTEGER REFERENCES Users (ID) ON DELETE CASCADE NOT NULL,
                ID_Command INTEGER REFERENCES Commands (ID) ON DELETE CASCADE NOT NULL,
                PRIMARY KEY (ID_User, ID_Command)
            );

            CREATE TABLE IF NOT EXISTS UserServers (
                ID_User   INTEGER REFERENCES Users (ID) ON DELETE CASCADE NOT NULL,
                ID_Server INTEGER REFERENCES Servers (ID) ON DELETE CASCADE NOT NULL,
                PRIMARY KEY (ID_User, ID_Server)
            );";
                    using (var command = new SqliteCommand(script, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при инициализации БД: {ex.Message}");
            }
        }

        private static async Task BackupDataBase()
        {
            try
            {
                if (!File.Exists(PathMain)) return;

                if (!Directory.Exists(PathBackup))
                {
                    Directory.CreateDirectory(PathBackup);
                }

                using (var connection = new SqliteConnection($"Data Source={PathMain}"))
                {
                    await connection.OpenAsync();
                    string currentBackupFilePath = Path.Combine(PathBackup, "DB-" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".db");

                    using (var connectionBackup = new SqliteConnection($"Data Source={currentBackupFilePath}"))
                    {
                        connection.BackupDatabase(connectionBackup);
                    }
                    LoggerService.MessageAppInfo("Бэкап прошел успешно: " + currentBackupFilePath);
                }
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при создании бэкапа: {ex.Message}");
            }
        }
    }
}
