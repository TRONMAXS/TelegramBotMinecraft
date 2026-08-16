using Microsoft.Data.Sqlite;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Core.Database
{
    public class DatabaseManager
    {
        private readonly string _connectionString;
        private readonly string _mainDbPath;
        private readonly string _backupDirectoryPath;
        private readonly LoggerService _loggerService;

        public DatabaseManager(string mainDbPath, string backupDirectoryPath, LoggerService loggerService)
        {
            _mainDbPath = mainDbPath;
            _backupDirectoryPath = backupDirectoryPath;
            _loggerService = loggerService;

            _connectionString = $"Data Source={_mainDbPath}";
        }

        public async Task Startup()
        {
            await BackupDataBase();
            DeleteOldBackups();
            await CheckOrCreateBD();
        }

        private async Task CheckOrCreateBD()
        {
            try
            {
                string? directory = Path.GetDirectoryName(_mainDbPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var enableForeignKeys = new SqliteCommand("PRAGMA foreign_keys = ON;", connection))
                    {
                        await enableForeignKeys.ExecuteNonQueryAsync();
                    }

                    string script = @"
CREATE TABLE IF NOT EXISTS Settings (
    ID            INTEGER PRIMARY KEY ON CONFLICT ROLLBACK
                          UNIQUE,
    BotToken      TEXT,
    AutoBot       INTEGER NOT NULL
                          DEFAULT (0),
    TrayOnStart   INTEGER NOT NULL
                          DEFAULT (0),
    RunAtStartup  INTEGER NOT NULL
                          DEFAULT (0),
    AutoReconnect INTEGER NOT NULL
                          DEFAULT (0),
    Notifications INTEGER NOT NULL
                          DEFAULT (0),
    ProxyHost     TEXT,
    ProxyPort     TEXT,
    ProxyUsername TEXT,
    ProxyPassword TEXT
);

CREATE TABLE IF NOT EXISTS Servers (
    ID          INTEGER PRIMARY KEY
                        NOT NULL
                        UNIQUE,
    Name        TEXT    NOT NULL
                        UNIQUE,
    Connected   TEXT,
    Path_Server TEXT,
    ID_Process  INTEGER DEFAULT ( -1),
    Java_Args   TEXT,
    Java_Name   TEXT    REFERENCES Java (Name) ON DELETE SET NULL
                                               ON UPDATE CASCADE,
    Rcon_Enable INTEGER DEFAULT (0),
    Rcon_Port   INTEGER DEFAULT (25575),
    Rcon_Pass   TEXT
);

CREATE TABLE IF NOT EXISTS Users (
    Name  TEXT    NOT NULL,
    ID_TG INTEGER NOT NULL,
    PRIMARY KEY (
        ID_TG
    )
    ON CONFLICT ROLLBACK
);

CREATE TABLE IF NOT EXISTS Commands (
    ID      INTEGER PRIMARY KEY
                    NOT NULL
                    UNIQUE,
    Command TEXT    UNIQUE
                    NOT NULL
);

CREATE TABLE IF NOT EXISTS Java (
    Name         TEXT UNIQUE
                      NOT NULL
                      PRIMARY KEY ON CONFLICT ROLLBACK,
    Version      TEXT UNIQUE
                      NOT NULL,
    Architecture TEXT,
    Path         TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS UserCommands (
    ID_User    INTEGER NOT NULL
                       REFERENCES Users (ID_TG) ON DELETE CASCADE
                                                ON UPDATE CASCADE,
    ID_Command INTEGER NOT NULL
                       REFERENCES Commands (ID) ON DELETE CASCADE,
    PRIMARY KEY (
        ID_User,
        ID_Command
    )
);

CREATE TABLE IF NOT EXISTS UserServers (
    ID_User   INTEGER NOT NULL
                      REFERENCES Users (ID_TG) ON DELETE CASCADE
                                               ON UPDATE CASCADE,
    ID_Server INTEGER NOT NULL
                      REFERENCES Servers (ID) ON DELETE CASCADE,
    PRIMARY KEY (
        ID_User,
        ID_Server
    )
);


";
            
                    using (var command = new SqliteCommand(script, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.ErrorAppInfo($"Ошибка при инициализации базы данных: {ex.Message}");
            }
        }

        private async Task BackupDataBase()
        {
            try
            {
                if (!File.Exists(_mainDbPath)) return;

                if (!Directory.Exists(_backupDirectoryPath))
                {
                    Directory.CreateDirectory(_backupDirectoryPath);
                }

                string backupFileName = $"Data-{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.db";
                string fullBackupFilePath = Path.Combine(_backupDirectoryPath, backupFileName);

                string backupConnectionString = $"Data Source={fullBackupFilePath}";

                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var connectionBackup = new SqliteConnection(backupConnectionString))
                    {
                        connection.BackupDatabase(connectionBackup);
                    }
                    _loggerService.MessageAppInfo("Бэкап базы данных прошел успешно: " + fullBackupFilePath);
                }
            }
            catch (Exception ex)
            {
                _loggerService.ErrorAppInfo($"Ошибка при создании бэкапа базы данных: {ex.Message}");
            }
        }

        public void DeleteOldBackups()
        {
            int daysToKeep = 7;
            try
            {
                if (!Directory.Exists(_backupDirectoryPath)) return;

                string[] backupFiles = Directory.GetFiles(_backupDirectoryPath, "Data-*.db");
                DateTime cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                int deletedCount = 0;

                foreach (string filePath in backupFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath); // Например, "Data-2026_08_10_01_02_52"

                    if (fileName.Length > 5)
                    {
                        string datePart = fileName.Substring(5); // Останется "2026_08_10_01_02_52"

                        if (DateTime.TryParseExact(datePart, "yyyy_MM_dd_HH_mm_ss",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out DateTime backupDate))
                        {
                            if (backupDate < cutoffDate)
                            {
                                File.Delete(filePath);
                                deletedCount++;
                            }
                        }
                    }
                }

                if (deletedCount > 0)
                {
                    _loggerService.MessageAppInfo($"Очистка бэкапов: успешно удалено {deletedCount} старых файлов.");
                }
            }
            catch (Exception ex)
            {
                _loggerService.ErrorAppInfo($"Ошибка при удалении старых бэкапов: {ex.Message}");
            }
        }
    }
}
