using CoreRCON;
using System.Net;
using TelegramBotMinecraft.Core.Database;

namespace TelegramBotMinecraft.Core.Services
{
    public class ServerCommandService(ServerRepository repository, MinecraftServerManager minecraftServerManager)
    {
        private readonly ServerRepository _repository = repository;
        private readonly MinecraftServerManager _minecraftServerManager = minecraftServerManager;

        public async Task<string> SendCommandToServer(string serverName, string command)
        {
            var ServerData = await _repository.GetServerByName(serverName);
            if (ServerData == null) return string.Empty;
            var Data = ServerData[0];
            command = command.Trim();
            try
            {
                if (Data.IdProcess != -1)
                {
                    if (Data.RconEnable == 1)
                    {
                        if (command == "stop")
                        {
                            bool response = await _minecraftServerManager.StopServer(serverName);
                            /*if (response == true) return string.Empty;
                            else return string.Empty;*/
                            return "Server Stopped";
                        }

                        using (var rcon = new RCON(IPAddress.Parse("127.0.0.1"), Convert.ToUInt16(Data.RconPort), Data.RconPass))
                        {
                            string response = await rcon.SendCommandAsync(command);
                            return response;
                        }
                    }
                    return "Rcon not available";
                }
                return "Server not running";
            }
            catch(Exception ex) { return ex.Message; }
        }
    }
}
