using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace rADiscordBotCore.Commands
{
    [Name("Help Commands")]
    public class HelpCommand : ModuleBase
    {
        CommandService commandService;
        string prefix;
        public HelpCommand()
        {
            commandService = rAthenaBot.instance.commands;
            prefix = rAthenaBot.instance.Config.PrefixChar;
        }

        [Command("help")]
        [Summary("Display help message")]
        public async Task HelpAsync(string command = null)
        {
            if (!rAthenaBot.instance.IsChannelWhitelisted(Context.Channel))
                return;

            if (command != null)
            {
                var result = commandService.Search(Context, command);

                if (!result.IsSuccess)
                {
                    await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                    return;
                }

                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description = $"Here are some commands like **{command}**"
                };

                foreach (var match in result.Commands)
                {
                    var cmd = match.Command;

                    builder.AddField(x =>
                    {
                        x.Name = string.Join(", ", cmd.Aliases);
                        x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                                  $"Summary: {cmd.Summary}";
                        x.IsInline = false;
                    });
                }

                await ReplyAsync("", false, builder.Build());
            }
            else
            {
                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description = $"These are the commands you can use. Use {prefix}help <command> to view a specific command's help."
                };

                foreach (var module in commandService.Modules)
                {
                    string description = null;
                    foreach (var cmd in module.Commands)
                    {
                        var result = await cmd.CheckPreconditionsAsync(Context);
                        if (result.IsSuccess)
                            description += $"{prefix}{cmd.Aliases.First()} - {cmd.Summary}\n";
                    }

                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        builder.AddField(x =>
                        {
                            x.Name = module.Name;
                            x.Value = description;
                            x.IsInline = false;
                        });
                    }
                }

                await ReplyAsync("", false, builder.Build());
            }
        }
    }
}
