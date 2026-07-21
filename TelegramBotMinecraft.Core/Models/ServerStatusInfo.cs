using static TelegramBotMinecraft.Core.Models.ServerStatusModel;

namespace TelegramBotMinecraft.Core.Models
{
    public class ServerStatusInfo
    {
        public string Server { get; set; } = string.Empty;
        public ServerStatus Status { get; set; } = ServerStatus.Offline;

        public ServerStatusInfo(string server, ServerStatus serverStatus)
        {
            Server = server;
            Status = serverStatus;
        }

    }
}
