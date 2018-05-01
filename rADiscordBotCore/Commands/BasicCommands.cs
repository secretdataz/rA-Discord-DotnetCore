using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace rADiscordBotCore.Commands
{
    public class BasicCommands : ModuleBase
    {
        [Command("stats"), Summary("Display server stats")]
        public async Task Stats()
        {
            using (Context.Channel.EnterTypingState()) {
                var users = await Context.Guild.GetUsersAsync();
                string msg = "**rAthena Discord Stat**:" + System.Environment.NewLine
                                    + "```"
                                    + "Region - " + Context.Guild.VoiceRegionId + System.Environment.NewLine
                                    + System.Environment.NewLine
                                    + "Total Users - " + users.Where(x => x.IsBot == false).Count() + System.Environment.NewLine
                                    + System.Environment.NewLine
                                    + "Total Bots - " + users.Where(x => x.IsBot == true).Count() + System.Environment.NewLine
                                    + "Total Online Bots - " + users.Where(x => x.IsBot == true && x.Status != UserStatus.Offline).Count() + System.Environment.NewLine
                                    + "Total Offline Bots - " + users.Where(x => x.IsBot == true && x.Status == UserStatus.Offline).Count() + System.Environment.NewLine
                                    + "```";
                await ReplyAsync(msg);
            }
        }

        [Command("search"), Summary("This command will search for contents at rAthena forum. ```Usage: !search <content>```")]
        [Alias("find")]
        public async Task Search([Remainder, Summary("Text to search")] string query)
        {
            await ReplyAsync("https://rathena.org/board/search/?&q=" + query.Replace(" ", "%20"));
        }

        [Command("purge"), Summary("This command will remove some Chat Log. ```Usage: !purge <amount> <@mention>```")]
        [Alias("remove", "clean", "delete")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge([Summary("Amount of messages to traverse back")] int amount, [Summary("The optional user to purge message")] IUser user = null)
        {
            if (amount > 0)
            {
                using (Context.Channel.EnterTypingState()) {
                    var msgs = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, amount).Flatten()).Where(msg => user == null || msg.Author.Id == user.Id);
                    await Context.Channel.DeleteMessagesAsync(msgs);
                    await ReplyAsync("Done purging " + msgs.Count() + " messages");
                }
            } else
            {
                await ReplyAsync("Amount is equal to or less than zero.");
            }            
        }

        [Command("whois")]
        [Alias("who", "profile", "check")]
        public async Task WhoIs(IUser user)
        {
            string msg = "**This command is WIP because the new API is...**"
                                        + "```"
                                        + "Name: " + user.Username + " (" + user.ToString() + ")" + System.Environment.NewLine
                                        //+ "Role: " + String.Join(",", user.Roles.Select(x => x.Name)) + System.Environment.NewLine
                                        + "Joined Since: " + user.CreatedAt + System.Environment.NewLine
                                        //+ "Last Activity: " + user.LastActivityAt + System.Environment.NewLine
                                        //+ "Last Online At: " + user.LastOnlineAt + System.Environment.NewLine
                                        //+ "Status: " + user.Status + System.Environment.NewLine
                                        + "```";
            await ReplyAsync(msg);
        }

        Dictionary<string, string> faqs = new Dictionary<string, string>
        {
            { "install", "*Installation guide*:\nhttps://github.com/rathena/rathena/wiki/installations" },
            { "contrib", "*How to contribute* report a bug or open a pull request:\nhttps://github.com/rathena/rathena/blob/master/.github/CONTRIBUTING.md" },
            { "forum", "*rAthena Forums*:\nhttps://rathena.org" },
            { "git", "*rAthena Git repository*:\nhttps://github.com/rathena/rathena" },
            { "flux", "*FluxCP Git repository*:\nhttps://github.com/rathena/fluxcp" },
            { "rasql", "*rASql Git repository*:\nhttps://github.com/rathena/rasql" },
            { "fluxdiscord", "*FluxCP Discord server*:\nhttps://discord.gg/JT3mD3t" },
            { "kroclient", "*kRO full client*:\nhttps://rathena.org/board/topic/106413-kro-full-client-2018-03-27-includes-bgm-rsu/" },
            { "grfeditor", "*Tokei's GRF Editor*:\nhttps://rathena.org/board/topic/77080-grf-grf-editor/" },
            { "thorpatcher", "*Aeomin's Thor patcher*:\nhttps://rathena.org/board/topic/59946-thor-patcher/" },
            { "nemo", "*NEMO client editor, 4144's fork*:\nhttps://gitlab.com/4144/Nemo" },
            { "donate", "*Donate to rAthena*, help us pay for hosting and stuffs:\nhttps://rathena.org/board/clients/donations/" },
            { "3ps", "*Third-party Services* paid service listing:\nhttps://rathena.org/thirdpartyservices/" },
            { "docsfolder", "*rAthena's docs folder* aka the rAthena Bible:\nhttps://github.com/rathena/rathena/tree/master/doc" }
        };
        string keys = "";
        [Command("faq"), Summary("This command will display the FAQ. ```Usage: !faq {<key>}```")]
        public async Task FAQ([Summary("The FAQ to be displayed")]string key = null)
        {
            if(key == null)
            {
                if(string.IsNullOrEmpty(keys))
                {
                    foreach (KeyValuePair<string, string> entry in faqs)
                    {
                        keys += entry.Key + " ";
                    }
                }

                await ReplyAsync("```Usage: !faq <key>\nAvailable keys are:\n" + keys + "```");
                return;
            }
            if (faqs.ContainsKey(key))
            {
                await ReplyAsync(faqs[key]);
            } else
            {
                await ReplyAsync("```Invalid FAQ key.\nnAvailable keys are:\n" + keys + "```");
            }
        }
    }
}
