using Nafi.Twitch;

public class Program
{
    const string OAUTH = "YOUR OAUTH TOKEN";
    const string CHANNEL = "your channel name";
    const string USERNAME = "Your Bot Username";

    static TwitchBot bot = new TwitchBot(OAUTH, CHANNEL, USERNAME);
    public static void Main(string[] args)
    {
        while (true)
        {
            TwitchChatMessage? chatMessage = null;

            chatMessage = bot.Read();

            if (chatMessage != null)
            {
                CheckCommand(chatMessage);
            }

        }
    }

    public static void CheckCommand(TwitchChatMessage chatMessage)
    {

        string msg = chatMessage.Message;
        string[] words = msg.Split(' ');

        switch (words[0].ToLower())
        {
            case "hello":
                bot.Write($"Hello {chatMessage.Sender}");
                break;
        }
    }

    public static void LurkCommand(TwitchChatMessage chatMessage)
    {
        bot.Write($"Thank you for lurking {chatMessage.Sender}");
    }


}
