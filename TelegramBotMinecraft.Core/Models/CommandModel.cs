namespace TelegramBotMinecraft.Core.Models
{
    public class Command
    {
        public int Id { get; set; }
        public string? CommandText { get; set; }

        public Command(int id, string command)
        {
            Id = id;
            CommandText = command;
        }
    }
}
