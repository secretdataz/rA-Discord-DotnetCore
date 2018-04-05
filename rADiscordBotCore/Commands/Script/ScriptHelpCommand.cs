using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace rADiscordBotCore.Commands.Script
{
    public class ScriptHelpCommand : ModuleBase
    {
        private const string fileLocation = "resources/script_commands.txt";
        private static List<ScriptCommand> scriptCommands;
        private static bool available = false;

        public static async Task<bool> ProcessScriptCommands()
        {
            await rAthenaBot.rABotLog(LogSeverity.Info, "Processing script_commands.txt");
            ScriptCommandParser scp;
            if (!File.Exists(fileLocation))
            {
                await rAthenaBot.rABotLog(LogSeverity.Info, "Downloading script_commands.txt from rAthena Git.");
                using (WebClient wc = new WebClient())
                {
                    await Task.Run(() => wc.DownloadFile(@"https://github.com/rathena/rathena/raw/master/doc/script_commands.txt", fileLocation));
                }
            }

            scp = new ScriptCommandParser(File.ReadAllLines(fileLocation));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Reading file...");
            Console.ForegroundColor = ConsoleColor.White;
            while (!scp.ready)
            {
                Thread.Sleep(3000);
                Console.Write(".");
            }
            Console.WriteLine();
            if (!scp.Parse())
            {
                await rAthenaBot.rABotLog(LogSeverity.Warning, "Processing failed. Script command doc will be disabled.");
                available = false;
                return false;
            }
            else
            {
                await rAthenaBot.rABotLog(LogSeverity.Info, "Processed script commands.");
                scriptCommands = scp.commands;
                available = true;
                return true;
            }
        }

        [Command("script"), Summary("View rAthena script command documentation.")]
        public async Task Script([Summary("Script command name")] string name)
        {
            if(!available)
            {
                await ReplyAsync("This command is not available.");
                return;
            }
            ScriptCommand cmd = scriptCommands.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (cmd == null)
            {
                await ReplyAsync("Command not found.");
            }
            else
            {
                string desc = "```";
                foreach (string x in cmd.Descriptions)
                {
                    desc += x + Environment.NewLine;
                }
                await ReplyAsync(desc + "```");
            }
        }
    }
}
