namespace TelegramBotMinecraft.Core.Models
{
    public class ServerStatusModel
    {
        public enum ServerStatus
        {
            Starting,
            Online,
            Warning,
            Crashed,
            Stopping,
            Offline
        }
    }
}
