using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;

namespace Nafi
{
    public class Twitch
    {
        bool exit = false;
        string channel;
        string nickname;

        TcpClient tcpClient;
        StreamReader inStream;
        StreamWriter outStream;

        Thread pingThread;

        public Twitch(string oauth_token, string channel, string nickname)
        {
            this.channel = channel;
            this.nickname = nickname;

            tcpClient = new TcpClient("irc.twitch.tv", 6667);
            inStream = new StreamReader(tcpClient.GetStream());
            outStream = new StreamWriter(tcpClient.GetStream());

            Console.WriteLine("CAP REQ :twitch.tv/command twitch.tv/tags twitch.tv/membership");
            outStream.WriteLine("CAP REQ :twitch.tv/command twitch.tv/tags twitch.tv/membership");
            outStream.WriteLine($"PASS {oauth_token}");
            Console.WriteLine("PASS {authorization token}");
            outStream.WriteLine($"NICK {nickname}");
            Console.WriteLine($"NICK {nickname}");
            outStream.WriteLine($"JOIN #{channel}");
            Console.WriteLine($"JOIN #{channel}");
            outStream.Flush();

            pingThread = new Thread(() => pingTwitch());
            pingThread.Start();
        }

        // Read a message coming in from Twitch chat and parse it.
        public TwitchChatMessage Read()
        {
            // Get Raw Message from chat
            string chatRaw;
            while (true)
            {
                if (exit) return null;

                chatRaw = inStream.ReadLine();
                Console.WriteLine(chatRaw);

                if (chatRaw == string.Empty)
                {
                    Thread.Sleep(100);
                    continue;
                }

                if (!chatRaw.Contains("PRIVMSG")) continue;
                break;
            }

            // Parse Raw Message into words by the ';' character
            string[] flags = chatRaw.Split(';');

            string colorRaw = string.Empty;
            string moderatorRaw = string.Empty;
            string subscriberRaw = string.Empty;
            string turboRaw = string.Empty;
            string firstMsgRaw = string.Empty;
            string messageRaw = string.Empty;


            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i].Contains("color="))
                {
                    colorRaw = flags[i];
                }
                if (flags[i].Contains("mod="))
                {
                    moderatorRaw = flags[i];
                }
                if (flags[i].Contains("turbo="))
                {
                    turboRaw = flags[i];
                }
                if (flags[i].Contains("subscriber="))
                {
                    subscriberRaw = flags[i];
                }
                if (flags[i].Contains("first-msg="))
                {
                    firstMsgRaw = flags[i];
                }
                if (flags[i].Contains("PRIVMSG"))
                {
                    messageRaw = flags[i];
                }
            }

            // Get Sender Color
            int colorStartIndex = colorRaw.IndexOf("=") + 1;
            string chatColorHex = colorRaw.Substring(colorStartIndex);
            string chatColor;
            switch (chatColorHex)
            {
                case "#FF0000":
                    chatColor = "Red";
                    break;
                case "#0000FF":
                    chatColor = "Blue";
                    break;
                case "#008000":
                    chatColor = "Green";
                    break;
                case "#B22222":
                    chatColor = "FireBrick";
                    break;
                case "#FF7F50":
                    chatColor = "Coral";
                    break;
                case "#DAA520":
                    chatColor = "Golden Rod";
                    break;
                case "#8A2BE2":
                    chatColor = "Blue Violet";
                    break;
                case "#D2691E":
                    chatColor = "Chocolate";
                    break;
                case "#5F9EA0":
                    chatColor = "Cadet Blue";
                    break;
                case "#9ACD32":
                    chatColor = "Yellow Green";
                    break;
                case "#2E8B57":
                    chatColor = "Sea Green";
                    break;
                case "#1E90FF":
                    chatColor = "Dodger Blue";
                    break;
                case "#00FF7F":
                    chatColor = "Spring Green";
                    break;
                case "#FF4500":
                    chatColor = "Orange Red";
                    break;
                case "#FF69B4":
                    chatColor = "Hot Pink";
                    break;
                default:
                    chatColor = "Error";
                    break;
            }

            // Get Subscriber Status
            int subcriberStartIndex = subscriberRaw.IndexOf("=") + 1;
            string chatSubscriber;
            string subscriberBoolean = subscriberRaw.Substring(subcriberStartIndex);
            if (subscriberBoolean == "0")
            {
                chatSubscriber = "False";
            }
            else if (subscriberBoolean == "1")
            {
                chatSubscriber = "True";
            }
            else
            {
                chatSubscriber = "Error";
            }

            // Get Mod Status
            int modStartIndex = moderatorRaw.IndexOf("=") + 1;
            string chatMod;
            string modBoolean = moderatorRaw.Substring(modStartIndex);
            if (modBoolean == "0")
            {
                chatMod = "False";
            }
            else if (modBoolean == "1")
            {
                chatMod = "True";
            }
            else
            {
                chatMod = "Error";
            }

            // Get Turbo Status
            int turboStartIndex = turboRaw.IndexOf("=") + 1;
            string chatTurbo;
            string turboBoolean = turboRaw.Substring(turboStartIndex);
            if (turboBoolean == "0")
            {
                chatTurbo = "False";
            }
            else if (turboBoolean == "1")
            {
                chatTurbo = "True";
            }
            else
            {
                chatTurbo = "Error";
            }

            // Get First Message Status
            int firstMessageStartIndex = firstMsgRaw.IndexOf("=") + 1;
            string chatFirstMessage;
            string firstMessageBoolean = firstMsgRaw.Substring(firstMessageStartIndex);
            if (firstMessageBoolean == "0")
            {
                chatFirstMessage = "False";
            }
            else if (firstMessageBoolean == "1")
            {
                chatFirstMessage = "True";
            }
            else
            {
                chatFirstMessage = "Error";
            }
            // nafiaus.tm
            int nameStartIndex = messageRaw.IndexOf("@") + 1;
            int nameEndIndex = messageRaw.IndexOf(":") - 4;
            string chatSender = messageRaw.Substring(nameStartIndex, nameEndIndex);

            // Get Message
            int messageIndexStart = messageRaw.IndexOf("#") + 1;
            string chatMessage = messageRaw.Substring(messageIndexStart + channel.Length + 2);

            return new TwitchChatMessage(chatRaw, chatColor, chatMod, chatSubscriber, chatTurbo, chatFirstMessage, chatSender, chatMessage);
        }


        // Write a message to Twitch chat using bot
        public bool Write(string message)
        {
            Console.WriteLine($":{nickname}!{nickname}@{nickname}.tmi.twitch.tv PRIVMSG #{channel} :{message}");
            return TcpWrite($":{nickname}!{nickname}@{nickname}.tmi.twitch.tv PRIVMSG #{channel} :{message}");
        }

        // Read a line from incoming from Twitch IRC server
        public string TcpRead()
        {
            return inStream.ReadLine();
        }

        // Write a line out to Twitch IRC server
        public bool TcpWrite(string message)
        {
            try
            {
                outStream.WriteLine(message);
                outStream.Flush();
                return true;
            }
            catch { return false; }
        }

        // Ping Twitch IRC server every once in a while to remain connected
        private void pingTwitch()
        {
            while (true)
            {
                if (exit) return;
                Console.WriteLine("PING irc.twitch.tv");
                var success = TcpWrite("PING irc.twitch.tv");

                if (!success)
                {
                    Thread.Sleep(100);
                    continue;
                }
                for (int i = 0; i < 300; i++)
                {
                    if (exit) return;
                    Thread.Sleep(1000);
                    i++;
                }
            }
        }

        // Memory cleaning function
        internal void Dispose()
        {
            Console.WriteLine($"Disposing {nickname}");
            exit = true;
            pingThread.Join();
            inStream.Dispose();
            outStream.Dispose();
            tcpClient.Dispose();
        }

    }
}
