using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace rADiscordBotCore.DivinePride
{
    public class DivinePrideCommands : ModuleBase
    {
        [Command("mobinfo"), Summary("This command will return information of a monster. ```Usage: !mobinfo <MonsterID>```")]
        [Alias("mi", "mobdb", "mobinfo")]
        public async Task MobInfo([Summary("Monster ID")]int mobid)
        {
            using (Context.Channel.EnterTypingState())
            {
                Monster mob = rAthenaBot.DpService.GetMonster(mobid);
                if (mob == null)
                {
                    await ReplyAsync("Can't retrieve monster info from divine-pride. Either mob-id doesn't exist or divine-pride is down.");
                }
                else
                {
                    string template = "Monster info for monster **{0} ({1})** :" + Environment.NewLine +
                                        "Stats: Lv:{2} HP:{3} SP:{4} STR:{5} AGI:{6} VIT:{7} INT:{8} DEX:{9} LUK:{10}" + Environment.NewLine +
                                        "Attack: {11}-{12} Def:{13} Mdef:{14} Exp:{15} JobExp:{16} Hit:{17} Flee:{18}" + Environment.NewLine +
                                        "Race:{19} Size:{20} Element:{21} MVP:{22}" + Environment.NewLine +
                                        "**Drops**: WIP";
                    await ReplyAsync(string.Format(template, mob.name, mob.kROName, mob.stats.level, mob.stats.health, mob.stats.sp,
                        mob.stats.str, mob.stats.agi, mob.stats.vit, mob.stats.Int, mob.stats.dex, mob.stats.luk, mob.stats.attack["minimum"], mob.stats.attack["maximum"],
                        mob.stats.defense, mob.stats.magicDefense, mob.stats.baseExperience, mob.stats.jobExperience, mob.stats.hit, mob.stats.flee, Monster.Idx2Race(mob.stats.race), Monster.Idx2Size(mob.stats.scale),
                        "WIP", mob.stats.mvp == 1 ? "Yes" : "No")); // TODO : Implement Element
                }
            }
        }

        [Command("ramob"), Summary("This command will return information of a monster in rAthena's mob_db.txt format.")]
        public async Task rAMob([Summary("Monster ID")]int mobid)
        {
            using (Context.Channel.EnterTypingState())
            {
                Monster mob = rAthenaBot.DpService.GetMonster(mobid);
                if (mob == null)
                {
                    await ReplyAsync("Can't retrieve monster info from divine-pride. Either mob-id doesn't exist or divine-pride is down.");
                }
                else
                {
                    await ReplyAsync("`" + mob.ToString() + "`");
                }
            }
        }

        [Command("raitem"), Summary("This command will return information of an item in rAthena's item_db.txt format.")]
        public async Task rAItem(int itemId)
        {
            using (Context.Channel.EnterTypingState())
            {
                Item item = rAthenaBot.DpService.GetItem(itemId);
                if (item == null)
                {
                    await ReplyAsync("Can't retrieve item info from divine-pride. Either item-id doesn't exist or divine-pride is down.");
                }
                else
                {
                    await ReplyAsync("`" + item.ToString() + "`");
                }
            }
        }
    }
}
