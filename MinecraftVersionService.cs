using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace TelegramBotMinecraft
{
    internal sealed class MinecraftVersionService
    {
        private string UrlManifestJson = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
        private HttpClient client = new HttpClient();

        public async Task MinecraftVersion()
        {
            try
            {
                string responseJson = await client.GetStringAsync(UrlManifestJson);

                var ManifestJson = JsonSerializer.Deserialize<MinecraftManifest>(responseJson);
                if (ManifestJson == null)
                    return;

                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    using var transaction = connection.BeginTransaction();


                    SqliteCommand command = new SqliteCommand("INSERT OR REPLACE INTO latest (id, release, snapshot) VALUES (1, @release, @snapshot);", connection);
                    command.Transaction = transaction;
                    command.Parameters.AddWithValue("@release", ManifestJson.latest.release);
                    command.Parameters.AddWithValue("@snapshot", ManifestJson.latest.snapshot);
                    await command.ExecuteNonQueryAsync();

                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText =
                    """
            INSERT OR IGNORE INTO versions
            (id, type, javaVersion, url)
            VALUES (@id, @type, @javaVersion, @url);
        """;

                    var pId = insertCommand.Parameters.Add("@id", SqliteType.Text);
                    var pType = insertCommand.Parameters.Add("@type", SqliteType.Text);
                    var pJava = insertCommand.Parameters.Add("@javaVersion", SqliteType.Integer);
                    var pUrl = insertCommand.Parameters.Add("@url", SqliteType.Text);

                    var existingVersions = new HashSet<string>();

                    var selectCommand = connection.CreateCommand();
                    selectCommand.Transaction = transaction;
                    selectCommand.CommandText = "SELECT id FROM versions";

                    using (var reader = await selectCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            existingVersions.Add(reader.GetString(0));
                        }
                    }

                    foreach (var version in ManifestJson.versions)
                    {
                        try
                        {
                            if (version.id == "13w43a") break;

                            if (existingVersions.Contains(version.id))
                                continue;

                            string responseJsonServerManifest = await client.GetStringAsync(version.url);
                            var ServerManifestJson = JsonSerializer.Deserialize<ServerManifest>(responseJsonServerManifest);
                            if (ServerManifestJson == null) continue;
                            int javaVersion = ServerManifestJson.javaVersion?.majorVersion ?? 8;

                            pId.Value = version.id;
                            pType.Value = version.type;
                            pJava.Value = javaVersion;
                            pUrl.Value = version.url;

                            await insertCommand.ExecuteNonQueryAsync();

                            Console.WriteLine(version.id);
                        }
                        catch (Exception ex) { Console.WriteLine($"{version.id}: {ex.Message}"); }
                    }
                    await transaction.CommitAsync();
                }
                Console.WriteLine("Данные занесены");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
    }

    public class MinecraftManifest
    {
        public Latest latest { get; set; }
        public Version[] versions { get; set; }
    }

    public class Latest
    {
        public string release { get; set; }
        public string snapshot { get; set; }
    }

    public class Version
    {
        public string id { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }

    public class ServerManifest
    {
        public Javaversion javaVersion { get; set; }
    }

    public class Javaversion
    {
        public int majorVersion { get; set; }
    }
}
