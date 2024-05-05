namespace Nafi.Twitch
{
    public class TwitchChatMessage
    {
        public readonly string Raw;
        public readonly string Color;
        public readonly string IsModerator;
        public readonly string IsSubscriber;
        public readonly string IsTurbo;
        public readonly string IsFirstMessage;
        public readonly string Sender;
        public readonly string Message;

        public TwitchChatMessage(string sender, string message)
        {
            this.Raw = sender + ":" + message;
            this.Color = string.Empty;
            this.IsModerator = string.Empty;
            this.IsSubscriber = string.Empty;
            this.IsTurbo = string.Empty;
            this.IsFirstMessage = string.Empty;
            this.Sender = sender;
            this.Message = message;
        }

        public TwitchChatMessage(string raw, string color, string mod, string subscriber, string turbo, string firstMessage, string sender, string message)
        {
            this.Raw = raw;
            this.Color = color;
            this.IsModerator = mod;
            this.IsSubscriber = subscriber;
            this.IsTurbo = turbo;
            this.IsFirstMessage = firstMessage;
            this.Sender = sender;
            this.Message = message;
        }
    }
}
