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

        public Server(int id, string name, string? connected, string? pathServer,
        int? idProcess, string? javaArgs, int? rconEnable, int? rconPort, string? rconPass)
        {
            Id = id;
            Name = name;
            Connected = connected;
            PathServer = pathServer;
            IdProcess = idProcess;
            JavaArgs = javaArgs;
            RconEnable = rconEnable;
            RconPort = rconPort;
            RconPass = rconPass;
        }

    }
}
