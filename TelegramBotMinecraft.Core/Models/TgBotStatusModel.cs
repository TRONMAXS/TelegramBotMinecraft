namespace TelegramBotMinecraft.Core.Models
{
    public class TgBotStatusModel
    {
        public enum TgBotStatus
        {
            Starting,
            Online,
            Warning,
            Reloading,
            Crashed,
            Stopping,
            Offline
        }
    }
}
