using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
