using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace rADiscordBotCore.Commands.Script
{
    [Name("rAthena script help commands")]
    public class ScriptHelpCommand : ModuleBase
    {
        private const string fileLocation = "resources/script_commands.txt";
        private const string scriptCmdTxtLocation = @"https://github.com/rathena/rathena/raw/master/doc/script_commands.txt";
        private const string updateCheckUrl = "https://api.github.com/repos/rathena/rathena/commits?path=doc/script_commands.txt";
        private const string versionLocation = "resources/script_commands.version";
        private static List<ScriptCommand> scriptCommands;
        private static bool available = false;

        private static DateTime LastRefresh = DateTime.MinValue;

        public static async Task<bool> ProcessScriptCommands()
        {
            await rAthenaBot.rABotLog(LogSeverity.Info, "Processing script_commands.txt");

            ScriptCommandParser scp = new ScriptCommandParser(File.ReadAllLines(fileLocation));

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
            if (!rAthenaBot.instance.IsChannelWhitelisted(Context.Channel))
                return;
            if (!available)
            {
                await ReplyAsync("This command is not available.");
                return;
            }

            bool shouldReprocess = await RefreshScriptCommandsTxt();
            if(shouldReprocess)
            {
                await ProcessScriptCommands();
            }

            ScriptCommand cmd = scriptCommands.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (cmd == null)
            {
                await ReplyAsync("Command not found. Do you meant any of these: " + String.Join(" ", scriptCommands.Where(x => x.Name.Contains(name)).Select(y => y.Name)));
            }
            else
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Script Command - " + cmd.Name);
                builder.AddInlineField(cmd.Descriptions.FirstOrDefault(), String.Join(Environment.NewLine, cmd.Descriptions.Skip(1)));
                builder.WithThumbnailUrl("https://dac.cssnr.com/static/images/logo.png");
                builder.Url = string.Format("https://github.com/rathena/rathena/blob/master/doc/script_commands.txt#L{0}-L{1}", cmd.LineNumberStart, cmd.LineNumberEnd);
                builder.WithColor(Color.LightGrey);

                await ReplyAsync(string.Empty, false, builder, RequestOptions.Default);
            }
        }

        private static async Task<string> GetNewestScriptCommandsTxtHash()
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("User-Agent", "C# rAthenaBot");
                string json = await wc.DownloadStringTaskAsync(updateCheckUrl);

                Regex rgx = new Regex("\"sha\":\"(\\w+)\"");
                Match match = rgx.Match(json);
                if (match.Groups.Count > 1)
                {
                    Group g = match.Groups[1];
                    if (g.Success)
                    {
                        return g.Value;
                    }
                }
            }

            throw new Exception("Failed to fetch script_commands.txt version.");
        }

        private static async Task<string> GetCurrentScriptCommandsHash()
        {
            string hash = "";
            if(File.Exists(versionLocation))
                await Task.Run(() => { hash = File.ReadAllText(versionLocation); });
            return hash;
        }

        public static async Task<bool> RefreshScriptCommandsTxt()
        {
            if(DateTime.Now.Subtract(LastRefresh).TotalMinutes < 1.0) // Throttle refresh rate!
            {
                return false;
            }

            string hash = await GetNewestScriptCommandsTxtHash();
            if(!GetCurrentScriptCommandsHash().Equals(hash))
            {
                await rAthenaBot.rABotLog(LogSeverity.Info, "Refreshing script_commands.txt version.");
                await Task.Run(() => {
                    File.Delete(fileLocation);
                    File.WriteAllText(versionLocation, hash);
                });
                await rAthenaBot.rABotLog(LogSeverity.Info, "Downloading new script_commands.txt from rAthena Git.");
                using (WebClient wc = new WebClient())
                {
                    await wc.DownloadFileTaskAsync(scriptCmdTxtLocation, fileLocation);
                }

                LastRefresh = DateTime.Now;
                return true;
            }

            return false;
        }
    }
}
