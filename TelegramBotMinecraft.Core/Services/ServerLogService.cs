using System.Runtime.CompilerServices;
using TelegramBotMinecraft.Core.Database;

namespace TelegramBotMinecraft.Core.Services
{
    public class ServerLogService(ServerRepository repository)
    {
        private readonly ServerRepository _repository = repository;

        public async IAsyncEnumerable<string> UpdateConsoleServer(string Name, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            string? pathToLogsServer = null;
            long lastPosition = 0;

            var server = await _repository.GetServerByName(Name);
            if (server == null || server.Count == 0)
            {
                yield return "Console log clear";
                yield break;
            }

            pathToLogsServer = Path.Combine(server[0].PathServer ?? string.Empty, "logs", "latest.log");
            if (!File.Exists(pathToLogsServer))
            {
                yield return "Console log clear";
                yield break;
            }

            try
            {
                using var fs = new FileStream(pathToLogsServer, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                using (var reader = new StreamReader(fs, System.Text.Encoding.UTF8, true, 1024, leaveOpen: true))
                {
                    string initialContent = await reader.ReadToEndAsync();
                    if (!string.IsNullOrEmpty(initialContent))
                    {
                        yield return initialContent;
                    }
                    lastPosition = fs.Position;



                    while (!cancellationToken.IsCancellationRequested)
                    {
                        long currentLength = fs.Length;

                        if (currentLength < lastPosition)
                        {
                            lastPosition = 0;
                            fs.Position = 0;
                            reader.DiscardBufferedData();
                            yield return "Console log clear";
                        }

                        if (currentLength > lastPosition)
                        {
                            fs.Position = lastPosition;
                            reader.DiscardBufferedData();

                            string content = await reader.ReadToEndAsync();
                            if (!string.IsNullOrEmpty(content) && !cancellationToken.IsCancellationRequested)
                            {
                                yield return content;
                            }
                            lastPosition = fs.Position;


                        }
                        await Task.Delay(1000, cancellationToken);
                    }
                }
            }
            finally { }
        }

    }
}
