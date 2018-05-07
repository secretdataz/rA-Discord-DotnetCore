using System.Collections.Generic;

namespace rADiscordBotCore.Commands.Script
{
    public class ScriptCommand
    {
        public string Name { get; set; }
        public List<string> Descriptions = new List<string>();
        public int LineNumberStart { get; set; }
        public int LineNumberEnd { get; set; }
    }
}
