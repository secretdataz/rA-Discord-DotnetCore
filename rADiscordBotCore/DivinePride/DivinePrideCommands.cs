using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace rADiscordBotCore.DivinePride
{
    [Name("Divine-Pride.org data display commands")]
    public class DivinePrideCommands : ModuleBase
    {
        [Command("mobinfo"), Summary("This command will return information of a monster.")]
        [Alias("mi", "mobdb", "mobinfo")]
        public async Task MobInfo([Summary("Monster ID")]int mobid)
        {
            if (!rAthenaBot.instance.IsChannelWhitelisted(Context.Channel))
                return;
            using (Context.Channel.EnterTypingState())
            {
                Monster mob = rAthenaBot.DpService.GetMonster(mobid);
                if (mob == null)
                {
                    await ReplyAsync("Can't retrieve monster info from divine-pride. Either mob-id doesn't exist or divine-pride is down.");
                }
                else
                {
                    using (Context.Channel.EnterTypingState())
                    {
                        EmbedBuilder builder = new EmbedBuilder();
                        builder.WithTitle("Monster Info - " + mob.name +" "+(mob.stats.mvp > 0 ? "(MVP)" : ""));
                        builder.AddInlineField("Level", mob.stats.level);
                        builder.AddInlineField("Max HP / SP", string.Format("HP - {0:N0} " + System.Environment.NewLine + "SP - {1:N0}", mob.stats.health, mob.stats.sp));
                        builder.AddInlineField("Attack", string.Format("Min - {0:N0} " + System.Environment.NewLine + "Max - {1:N0}", mob.stats.attack["minimum"], mob.stats.attack["maximum"]));
                        builder.AddInlineField("Defense", string.Format("Def - {0:N0} " + System.Environment.NewLine + "MDef - {1:N0}", mob.stats.defense, mob.stats.magicDefense));
                        builder.AddInlineField("EXP", string.Format("Base - {0:N0} " + System.Environment.NewLine + "Job - {1:N0}", mob.stats.baseExperience, mob.stats.jobExperience));
                        builder.AddInlineField("Hit", mob.stats.hit);
                        builder.AddInlineField("Flee", mob.stats.flee);
                        builder.AddInlineField("Race", Monster.Idx2Race(mob.stats.race));
                        builder.AddInlineField("Size", Monster.Idx2Size(mob.stats.scale));
                        // builder.AddInlineField("Element", Monster.Idx2Element(mob.stats.element)); // WIP
                        // builder.AddInlineField("Drops", "Apple")); // WIP
                        builder.AddInlineField("Status", string.Format("STR - {0} / AGI - {1} / VIT - {2} / INT - {3} / DEX - {4} / LUK - {5}", mob.stats.str, mob.stats.agi, mob.stats.vit, mob.stats.Int, mob.stats.dex, mob.stats.luk));
                    
                        builder.WithThumbnailUrl("https://static.divine-pride.net/images/mobs/png/" + mobid + ".png");
                        builder.Url = "https://www.divine-pride.net/database/monster/" + mobid + "/";
                        builder.WithAuthor("Divine Pride", "https://i.imgur.com/2qSoMlF.jpg", "https://www.divine-pride.net/");
                        builder.WithColor(Color.LightOrange);

                        await ReplyAsync(string.Empty, false, builder, RequestOptions.Default);
                    }
                }
            }
        }

        [Command("iteminfo"), Summary("This command will return information of an item.")]
        [Alias("ii", "itemdb", "iteminfo")]
        public async Task ItemInfo([Summary("Item ID")]int itemid)
        {
            if (!rAthenaBot.instance.IsChannelWhitelisted(Context.Channel))
                return;
            using (Context.Channel.EnterTypingState())
            {
                Item item = rAthenaBot.DpService.GetItem(itemid);
                if (item == null)
                {
                    await ReplyAsync("Can't retrieve item info from divine-pride. Either item-id doesn't exist or divine-pride is down.");
                }
                else
                {
                    using (Context.Channel.EnterTypingState())
                    {
                        string itemDesc = item.description;
                        int i = -1;
                        while ((i = itemDesc.IndexOf("^")) >= 0)
                        {
                            itemDesc = itemDesc.Remove(i, 7); // Remove color code
                        }

                        EmbedBuilder builder = new EmbedBuilder();
                        builder.WithTitle("Item Info - " + item.name+" [" + item.slots.GetValueOrDefault() + "]");
                        builder.AddInlineField("Item Type", Item.Idx2ItemType(item.itemTypeId.GetValueOrDefault()));
                        builder.AddInlineField("Item Sub-Type", item.itemSubTypeId.GetValueOrDefault());
                        builder.AddInlineField("MATK", item.matk.GetValueOrDefault());
                        builder.AddInlineField("ATK", item.attack.GetValueOrDefault());
                        builder.AddInlineField("Defense", item.defense.GetValueOrDefault());
                        builder.AddInlineField("Attribute", item.attribute.GetValueOrDefault());
                        builder.AddInlineField("Range", item.range.GetValueOrDefault());
                        builder.AddInlineField("Weight", item.weight.ToString());
                        builder.AddInlineField("Limit Level", item.limitLevel.GetValueOrDefault());
                        builder.AddInlineField("Weapon Level", item.weaponLevel.GetValueOrDefault());
                        builder.AddInlineField("Price", item.price.GetValueOrDefault());
                        builder.AddInlineField("Job", item.job.GetValueOrDefault());
                        builder.AddInlineField("Gender", item.gender.GetValueOrDefault());
                        //builder.AddInlineField("Location", item.location.ToString());
                        builder.AddInlineField("Refineable", item.refinable.GetValueOrDefault());
                        builder.AddInlineField("Indestructible", item.indestructible.GetValueOrDefault());
                        builder.AddInlineField("View ID", item.classNum.GetValueOrDefault());
                        builder.AddInlineField("Description", itemDesc.ToString());

                        if (item.itemTypeId.GetValueOrDefault() == 6) // Card Type
                        {
                            builder.WithThumbnailUrl("https://static.divine-pride.net/images/items/cards/" + itemid + ".png");
                        }
                        else
                        {
                            builder.WithThumbnailUrl("https://www.divine-pride.net/img/items/collection/iRO/" + itemid);
                        }
                        builder.Url = "https://www.divine-pride.net/database/item/" + itemid + "/";
                        builder.WithColor(Color.LightOrange);
                        builder.WithAuthor("Divine Pride", "https://i.imgur.com/2qSoMlF.jpg", "https://www.divine-pride.net/");

                        await ReplyAsync(string.Empty, false, builder, RequestOptions.Default);
                    }
                }
            }
        }

        [Command("ramob"), Summary("This command will return information of a monster in rAthena's mob_db.txt format.")]
        public async Task rAMob([Summary("Monster ID")]int mobid)
        {
            if (!rAthenaBot.instance.IsChannelWhitelisted(Context.Channel))
                return;
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
            if (!rAthenaBot.instance.IsChannelWhitelisted(Context.Channel))
                return;
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
