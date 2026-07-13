namespace TelegramBotMinecraft.Core.Models
{
    public class Server
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Connected { get; set; }
        public string? PathServer { get; set; }
        public int? IdProcess { get; set; } = -1;
        public string? JavaArgs { get; set; }
        public int? RconEnable { get; set; } = 0;
        public int? RconPort { get; set; }
        public string? RconPass { get; set; }

        public Server(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
