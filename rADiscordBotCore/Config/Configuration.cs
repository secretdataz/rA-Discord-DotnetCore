using System;
using System.Collections.Generic;

namespace rADiscordBotCore
{
    public class Configuration
    {
        public string ConsoleTitle { get; set; }
        public string DiscordToken { get; set; }
        public ulong ServerId { get; set; }
        public string PrefixChar { get; set; }
        public bool AllowMentionPrefix { get; set; }
        public Dictionary<String,String> Channels { get; set; }
        public string DivinePrideApiKey { get; set; }
        public string DivinePrideBaseUrl { get; set; }
    }
}
