using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft.Core.Services
{
    public interface IWindowService
    {
        Task OpenJavaManagement();

        Task<string> OpenJavaToServerArgumentManagement(Server? server);
    }
}
