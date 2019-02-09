using Discord;
using Discord.Commands;
using Discord.WebSocket;
using rADiscordBotCore.Commands.Script;
using rADiscordBotCore.DivinePride;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace rADiscordBotCore
{
    partial class rAthenaBot
    {
        public static rAthenaBot instance { get; private set; }
        public Configuration Config { get; private set; }
        DiscordSocketClient discord;
        CommandService commands;
        RSSConfiguration RSSConfig;
        public static DivinePrideService DpService;
        Dictionary<String, SocketTextChannel> Channels = new Dictionary<string, SocketTextChannel>();
        private IServiceProvider services;

        System.Timers.Timer TimerRSS = new System.Timers.Timer();
        long Tick_RSS_Support = DateTime.Now.Ticks;
        long Tick_RSS_Server = DateTime.Now.Ticks;

        private static Task Log(LogMessage msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[" + DateTime.Now.ToString() + "] Log - " + msg.ToString());
            Console.ForegroundColor = ConsoleColor.White;
            return Task.CompletedTask;

        }

        public static async Task rABotLog(LogSeverity severity, string msg)
        {
            await Log(new LogMessage(severity, "rAthenaBot", msg));
        }

        public rAthenaBot()
        {
            
        }

        public async Task MainAsync()
        {
            services = new ServiceCollection().BuildServiceProvider();

            await rABotLog(LogSeverity.Info, "Reading configuration files.");
            #region Process configurations
            string _Result = ProcessConfig();
            if (!String.IsNullOrEmpty(_Result))
            {
                await rABotLog(LogSeverity.Critical, "Configuration file " + _Result + " is missing."); // ERROR_OPEN_FAILED                
                Environment.Exit(1);
            }
            #endregion
            else
            {
                await rABotLog(LogSeverity.Info, "Done reading configuration files.");
                Console.Title = Config.ConsoleTitle;

                DpService = new DivinePrideService
                {
                    BaseUrl = Config.DivinePrideBaseUrl,
                    ApiKey = Config.DivinePrideApiKey
                };

                discord = new DiscordSocketClient();
                discord.Log += Log;

                #region Restrict bot to rAthena server
                discord.JoinedGuild += async (server) =>
                {
                    if(server.Id != Config.ServerId)
                    {
                        await server.LeaveAsync();
                    } else
                    {
                        foreach(var chanStr in Config.Channels)
                        {
                            var channel = server.TextChannels.FirstOrDefault((e) => e.Name.Equals(chanStr.Value));
                            if(channel == null)
                            {
                                await rABotLog(LogSeverity.Critical, chanStr.Value + " channel doesn't exists. Exiting...");
                                Environment.Exit(1);
                            }
                        }

                        await Channels["General"].SendMessageAsync("Test message please ignore. Testing CygnusBot for .NET Core");
                    }
                };
                #endregion

                #region Discord Events - UserJoined
                discord.UserJoined += async (user) =>
                {
                    await Channels["General"].SendMessageAsync("Hello " + user.Mention + ", welcome to rAthena Discord." + Environment.NewLine +
                                "Kindly read the " + Channels["Rules"].Mention + " before you start posting. Thank you.");
                };
                #endregion

                #region Register Commands
                commands = new CommandService();
                discord.MessageReceived += HandleCommand;
                await commands.AddModulesAsync(Assembly.GetEntryAssembly());
                #endregion

                await ScriptHelpCommand.RefreshScriptCommandsTxt();
                if(!(await ScriptHelpCommand.ProcessScriptCommands()))
                {
                    await Log(new LogMessage(LogSeverity.Warning, "rAthenaBot", "Script command process failed. Script command help will not be available!"));
                }

                #region RSS Timer
                /*TimerRSS.Interval = RSSConfig.RefreshInterval;
                TimerRSS.AutoReset = RSSConfig.AutoReset;
                TimerRSS.Enabled = RSSConfig.Enabled;
                TimerRSS.Elapsed += new ElapsedEventHandler(OnCheckRSSFeed);
                TimerRSS.Start();*/
                #endregion

                instance = this;
                await discord.LoginAsync(TokenType.Bot, Config.DiscordToken);
                await discord.StartAsync();
                await discord.SetGameAsync("rAthena", "https://www.rAthena.org", StreamType.NotStreaming);
                await discord.SetStatusAsync(UserStatus.Online);

                await Task.Delay(-1);
            }
        }

        private string ProcessConfig()
        {
            string _MainConfigFile = "resources/config.json";
            string _RSSConfigFile = "resources/config_rss.json";
            if (!File.Exists(_MainConfigFile))
            {
                return _MainConfigFile;
            }
            else if (!File.Exists(_RSSConfigFile))
            {
                return _RSSConfigFile;
            }
            Config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(_MainConfigFile));
            RSSConfig = JsonConvert.DeserializeObject<RSSConfiguration>(File.ReadAllText(_RSSConfigFile));
            return string.Empty;
        }

        #region Rich Site Summary 
        /*private void OnCheckRSSFeed(object source, ElapsedEventArgs e)
        {
            discord.ExecuteAndWait(async () =>
            {
                TimerRSS.Stop();
                try
                {
                    string message = string.Empty;
                    #region Search for New Support RSS
                    Channel supportChannel = discord.FindServers(Config.ServerName).FirstOrDefault().FindChannels(Config.Channels["Support"], ChannelType.Text).FirstOrDefault();

                    if (supportChannel != null && RSSConfig.SupportFeeds != null && RSSConfig.SupportFeeds.Count > 0)
                    {
                        List<Tuple<string, string, long>> SupportRSSList = new List<Tuple<string, string, long>>();
                        List<string> SupportFeeds = RSSConfig.SupportFeeds;

                        foreach (string rss in SupportFeeds)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load(rss);
                            XmlNodeList elemList = doc.GetElementsByTagName("item");
                            for (int i = 0; i < elemList.Count; i++)
                            {
                                XmlNodeList node = elemList[i].ChildNodes;
                                long timetick = DateTime.Parse(node[(byte)Constant.SupportFeed.PublishDate].InnerText).Ticks;
                                if (timetick > Tick_RSS_Support)
                                {
                                    SupportRSSList.Add(new Tuple<string, string, long>(
                                        node[(byte)Constant.SupportFeed.Title].InnerText,
                                        node[(byte)Constant.SupportFeed.Link].InnerText,
                                        timetick));
                                }
                            }
                        }

                        if (SupportRSSList.Count > 0)
                        {
                            await supportChannel.SendIsTyping();

                            message = "**I've found " + SupportRSSList.Count + " New Topic(s), anybody want to take a look at it? **";
                            foreach (Tuple<string, string, long> rss in SupportRSSList)
                            {
                                message = message + System.Environment.NewLine
                                        //+ rss.Item1 + System.Environment.NewLine 
                                        + rss.Item2 + System.Environment.NewLine;
                            }
                            await supportChannel.SendMessage(message);
                            Tick_RSS_Support = DateTime.Now.Ticks;
                        }
                    }
                    #endregion

                    #region Search for New Server RSS
                    //Channel serverChannel = discord.FindServers(Config.ServerName).FirstOrDefault().FindChannels(Config.Channels["ServerAds"], ChannelType.Text).FirstOrDefault();

                    //if (serverChannel != null && RSSConfig.ServerFeeds != null && RSSConfig.ServerFeeds.Count > 0)
                    //{
                    //    List<Tuple<string, string, long>> ServerRSSList = new List<Tuple<string, string, long>>();
                    //    List<string> ServerFeeds = RSSConfig.ServerFeeds;

                    //    foreach (string rss in ServerFeeds)
                    //    {
                    //        XmlDocument doc = new XmlDocument();
                    //        doc.Load(rss);
                    //        XmlNodeList elemList = doc.GetElementsByTagName("item");
                    //        for (int i = 0; i < elemList.Count; i++)
                    //        {
                    //            XmlNodeList node = elemList[i].ChildNodes;
                    //            long timetick = DateTime.Parse(node[(byte)Constant.ServerFeed.PublishDate].InnerText).Ticks;
                    //            if (timetick > Tick_RSS_Server)
                    //            {
                    //                ServerRSSList.Add(new Tuple<string, string, long>(
                    //                    node[(byte)Constant.ServerFeed.Title].InnerText,
                    //                    node[(byte)Constant.ServerFeed.Link].InnerText,
                    //                    timetick));
                    //            }
                    //        }
                    //    }
                    
                    //    if (ServerRSSList.Count > 0)
                    //    {
                    //        await serverChannel.SendIsTyping();
                    //        foreach (Tuple<string, string, long> rss in ServerRSSList)
                    //        {
                    //            message = rss.Item2 + System.Environment.NewLine;
                    //            await serverChannel.SendMessage(message);
                    //        }
                    //        Tick_RSS_Server = DateTime.Now.Ticks;
                    //    }
                    //}
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] Exception : " + ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                TimerRSS.Start();
            });
        }*/
        #endregion

        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasStringPrefix(Config.PrefixChar, ref argPos) || message.HasMentionPrefix(discord.CurrentUser, ref argPos))) return;

            // Create a Command Context
            var context = new CommandContext(discord, message);

            var name = message.Content.Substring(1);
            if (File.Exists("resources/img/" + name + ".gif"))
            {
                using (context.Channel.EnterTypingState())
                {
                    // Don't have to await [Secret]
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    context.Channel.SendFileAsync("resources/img/" + name + ".gif");
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    return;
                }
            }
            else
            {
                // Execute the command. (result does not indicate a return value, 
                // rather an object stating if the command executed successfully)
                var result = await commands.ExecuteAsync(context, argPos, services);
                if (!result.IsSuccess)
                    await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
 
