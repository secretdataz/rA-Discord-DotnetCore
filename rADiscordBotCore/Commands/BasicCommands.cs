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
            using (Context.Channel.EnterTypingState())
            {
                var users = await Context.Guild.GetUsersAsync();

                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("rAthena Discord Stats");
                builder.AddInlineField("Discord Name", Context.Guild.Name);
                builder.AddInlineField("Region", Context.Guild.VoiceRegionId);
                builder.AddInlineField("Verification Level", Context.Guild.VerificationLevel);
                builder.AddInlineField("Established Since", Context.Guild.CreatedAt.ToString("dd-MMM-yyyy"));
                builder.AddInlineField("Bot Status", string.Format("{0} Online / {1} Offline",
                    users.Where(x => x.IsBot == true && x.Status == UserStatus.Online).Count(),
                    users.Where(x => x.IsBot == true && x.Status == UserStatus.Offline).Count()));
                builder.AddInlineField("Webhook Status", string.Format("{0} Online / {1} Offline", 
                    users.Where(x => x.IsWebhook == true && x.Status == UserStatus.Online).Count(), 
                    users.Where(x => x.IsWebhook == true && x.Status == UserStatus.Offline).Count()));
                builder.AddInlineField("User Status", string.Format("{0} Online / {1} Offline / {2} Idle / {3} AFK / {4} DoNotDisturb / {5} Invisible",
                    users.Where(x => x.IsBot == false && x.IsWebhook == false && x.Status == UserStatus.Online).Count(),
                    users.Where(x => x.IsBot == false && x.IsWebhook == false && x.Status == UserStatus.Offline).Count(),
                    users.Where(x => x.IsBot == false && x.IsWebhook == false && x.Status == UserStatus.Idle).Count(),
                    users.Where(x => x.IsBot == false && x.IsWebhook == false && x.Status == UserStatus.AFK).Count(),
                    users.Where(x => x.IsBot == false && x.IsWebhook == false && x.Status == UserStatus.DoNotDisturb).Count(),
                    users.Where(x => x.IsBot == false && x.IsWebhook == false && x.Status == UserStatus.Invisible).Count()));
                builder.WithThumbnailUrl("https://dac.cssnr.com/static/images/logo.png");
                builder.Url = "http://rathena.org";
                builder.WithColor(Color.Green);

                await ReplyAsync(string.Empty, false, builder, RequestOptions.Default);
            }
        }
        
        [Command("search"), Summary("This command will search for contents at rAthena forum. ```Usage: !search <content>```")]
        [Alias("find")]
        public async Task Search([Remainder, Summary("Text to search")] string query)
        {
            using (Context.Channel.EnterTypingState())
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithAuthor("rAthena Forum - Search Engine", "https://i.imgur.com/sRTCmlX.jpg", "https://rathena.org/board/search/?&q=" + query.Replace(" ", "%20"));
                builder.WithTitle("Query: " + query);
                builder.AddInlineField("Description", "The information may be a mix of various answer for similar questions.");
                builder.WithThumbnailUrl("https://i.imgur.com/sRTCmlX.jpg"); // rAthenaLogo
                builder.Url = "https://rathena.org/board/search/?&q=" + query.Replace(" ", "%20");
                builder.WithColor(Color.LightOrange);

                await ReplyAsync(string.Empty, false, builder, RequestOptions.Default);
            }
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
                    
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl(), "http://rathena.org");
                    builder.WithTitle("Purged " + msgs.Count() + " message(s)" + (user == null ? "" : " posted by " + user.Username.ToString()) + ".");
                    builder.WithThumbnailUrl("https://i.imgur.com/eTFbzF1.png"); // Delete Icon
                    builder.WithColor(Color.Red);

                    await ReplyAsync(string.Empty, false, builder, RequestOptions.Default);
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
            using (Context.Channel.EnterTypingState())
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Basic User Profile");
                builder.AddInlineField("Discord ID", user.ToString());
                builder.AddInlineField("Discord Name", user.Username.ToString());
                builder.AddInlineField("Playing Game", (user.Game.HasValue ? user.Game.ToString() : "None"));
                builder.AddInlineField("Joined Since", user.CreatedAt.ToString("dd-MMM-yyyy"));
                builder.AddInlineField("Status", user.Status.ToString());
                builder.WithThumbnailUrl(user.GetAvatarUrl());
                builder.WithColor(Color.Gold);

                await ReplyAsync(string.Empty, false, builder, RequestOptions.Default);
            }
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
