using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace rADiscordBotCore.DivinePride
{
    public partial class Item
    {
        public int id { get; set; }
        public string aegisName { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int? slots { get; set; }
        public int? itemTypeId { get; set; }
        public int? itemSubTypeId { get; set; }
        public int? matk { get; set; }
        public int? attack { get; set; }
        public int? defense { get; set; }
        public int? attribute { get; set; }
        public int? range { get; set; }
        public string weight { get; set; }
        public int? limitLevel { get; set; }
        public int? weaponLevel { get; set; }
        public int? price { get; set; }
        public int? job { get; set; }
        public int? gender { get; set; }
        public string location { get; set; }
        public bool? refinable { get; set; }
        public bool? indestructible { get; set; }
        public int? classNum { get; set; }

        public ItemMoveInfo itemMoveInfo { get; set; }
        public class ItemMoveInfo
        {
            public bool drop { get; set; }
            public bool trade { get; set; }
            public bool store { get; set; }
            public bool cart { get; set; }
            public bool sell { get; set; }
            public bool mail { get; set; }
            public bool auction { get; set; }
            public bool guildStore { get; set; }
        }

        public List<ItemSetList> sets { get; set; }
        public class ItemSetList
        {
            public string name { get; set; }
            public List<ItemSet> items { get; set; }
        }

        public class ItemSet
        {
            public int itemId { get; set; }
            public string name { get; set; }
        }
        
        public List<SoldByInfo> soldBy { get; set; }
        public class SoldByInfo
        {
            public SoldByInfoSub npc { get; set; }
            public int price { get; set; }

            public class SoldByInfoSub
            {
                public int id { get; set; }
                public string name { get; set; }
                public string mapname { get; set; }
                public int? job { get; set; }
                public int? x { get; set; }
                public int? y { get; set; }
                public string type { get; set; }
            }
        }
        
        public string setname { get; set; }

        public List<ItemSummonInfo> itemSummonInfoContainedIn { get; set; }
        public class ItemSummonInfo
        {
            public int Type { get; set; }
            public int sourceId { get; set; }
            public string sourceName { get; set; }
            public int targetId { get; set; }
            public string targetName { get; set; }
            public int count { get; set; }
            public int totalOfSource { get; set; }
            public string summonType { get; set; }
            public int chance { get; set; }
        }

        public List<int> itemSummonInfoContains { get; set; }
        public int? requiredLevel { get; set; }
        public int? compositionPos { get; set; }
        public string accessory { get; set; }
        public List<string> rewardForAchievement { get; set; }
        public string cardPrefix { get; set; }
        public List<string> pets { get; set; }

        public static string Idx2ItemType(int idx)
        {
            switch (idx)
            {
                case 0:
                    return "Healing";
                case 2:
                    return "Usable";
                case 3:
                    return "Etc";
                case 4:
                    return "Equipment";
                case 5:
                    return "Weapon";
                case 6:
                    return "Card";
                case 7:
                    return "Pet Egg";
                case 8:
                    return "Pet Equipment";
                case 10:
                    return "Ammo";
                case 11:
                    return "Usable (Delayed consumption)";
                case 12:
                    return "Shadow Equipment";
                case 18:
                    return "Usable (Delayed consumption)";
                default:
                    return "Unknown";
            }
        }
    }

}
