namespace TelegramBotMinecraft.Core.Models
{
    public partial class User
    {
        public string Name { get; set; }

        public long Id { get; set; }

        public User(string name, long id)
        {
            Name = name;
            Id = id;
        }
    }
}
