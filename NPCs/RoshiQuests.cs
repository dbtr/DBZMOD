﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace DBZMOD.NPCs
{
    //lots of this code was restructured from antiaris with love <3
    public class RoshiQuests : ModPlayer
    {
        public static List<Quest> Quests = new List<Quest>();
        public int QuestsCompleted = 0;
        public int QuestsCompletedToday = 0;
        public int QuestLimitPerDay = 3;
        public int CurrentQuest = 0;

        public override void Initialize()
        {

            Quests.Clear();

            Quest quest = new ItemQuest("Oh, its you. I could use some assistance with something if you have the time. I just need some gel from the slimes in the forest and don't ask why, I need it for my 'research', anyways 20 should be good. Don't worry, I'll reward you appropriately for your efforts.", ItemID.Gel, 20, 1, "Thanks for your assistance, take these odd crystals I found earlier while I was lost in a forest.")
            {
                QuestReward = "StableKiCrystal",
                QuestRewardAmount = 25
            };
            Quests.Add(quest);

            quest = new ItemQuest("Ah, welcome back. I could use more of your assistance if you have the time. I'd like if you could bring me one of those shackles that these mutant things hold, it'll help with my 'research'.", ItemID.Shackle, 1, 1, "Thanks for your assistance, you can have a few of these ki restoration potions I've held onto forever, they should come in handy.")
            {
                QuestReward = "KiPotion2",
                QuestRewardAmount = 6
            };
            Quests.Add(quest);

            //Kill quest example
            /*int[] miner = { NPCID.UndeadMiner };
            quest = new KillQuest(Name8 + "\n\n" + Quest8, miner, 1, 1d);
            quest.Reward = "";
            Quests.Add(quest);*/




        }

        public override void Load(TagCompound tag)
        {
            CurrentQuest = tag.GetInt("CurrentQuest");
            QuestsCompletedToday = tag.GetInt("QuestsCompletedToday");
            QuestsCompleted = tag.GetInt("QuestsCompleted");
        }

        public override TagCompound Save()
        {
            var tag = new TagCompound();
            tag.Set("CurrentQuest", CurrentQuest);
            tag.Set("QuestsCompletedToday", QuestsCompletedToday);
            tag.Set("QuestsCompleted", QuestsCompleted);
            return tag;
        }


        public Quest GetCurrentQuest()
        {
            return Quests[CurrentQuest];
        }

        public void CompleteQuest()
        {
            QuestsCompletedToday++;
            QuestsCompleted++;
            CurrentQuest = 0;
        }

        public void SpawnReward(NPC npc)
        {
            Main.PlaySound(24, -1, -1, 1);
            if (GetCurrentQuest().QuestReward == "")
            {
                return;
            }
                
            int number2 = 0;
            number2 = Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height,
                mod.ItemType(GetCurrentQuest().QuestReward), GetCurrentQuest().QuestRewardAmount, false, 0, false, false);
            if (Main.netMode == 1 && number2 >= 0)
                NetMessage.SendData(21, -1, -1, null, number2, 1f, 0.0f, 0.0f, 0, 0, 0);
        }

        public override void PostUpdate()
        {
            if (Main.dayTime && QuestsCompletedToday > 0)
            {
                if (Main.time < 1 || (Main.fastForwardTime && Main.time < 61))
                {
                    CurrentQuest = 0;
                    QuestsCompletedToday = 0;
                }
            }
        }
        
        public static int ChooseNewQuest()
        {
            int questChoice = 0;
            RoshiQuests roshiQuests = new RoshiQuests();
            for (int i = 0; i < Quests.Count; i++)
            {
                if (Quests[i].IsAvailable())
                {
                    if (i <= roshiQuests.QuestsCompleted)
                    {
                        questChoice = i;
                    }
                }
                    
            }
            return questChoice;
        }
        public bool CheckQuest()
        {
            if (CurrentQuest == -1)
                return false;

            var quest = Quests[CurrentQuest];
            return quest.CheckCompletion(Main.player[Main.myPlayer]);
        }
    }

    

    public abstract class Quest
    {
        public Func<bool> IsAvailable;
        public string QuestName;
        public string QuestReward = "";
        public int QuestRewardAmount = 1;
        public string ThanksMessage;
        public double Weight;

        protected Quest(string name, double weight = 1d, string specialThanks = "Thanks for your assistance!")
        {
            QuestName = name;
            Weight = weight;
            ThanksMessage = specialThanks;
            IsAvailable = () => true;
        }

        public abstract bool CheckCompletion(Player player);

        public override string ToString()
        {
            return Language.GetTextValue(QuestName, Main.LocalPlayer.name);
        }

        public string SayThanks()
        {
            return Language.GetTextValue(ThanksMessage, Main.LocalPlayer.name);
        }
    }

    public class ItemQuest : Quest
    {
        public int ItemAmount;
        public int ItemType;

        public ItemQuest(string name, int itemType, int itemAmount = 1, double weight = 1d, string specialThanks = "Thanks for your assistance! Here's the promised reward.") : base(name, weight, specialThanks)
        {
            ItemType = itemType;
            ItemAmount = itemAmount;
        }

        public override bool CheckCompletion(Player player)
        {
            if (player.CountItem(ItemType, ItemAmount) >= ItemAmount)
            {
                int leftToRemove = ItemAmount;
                foreach (var item in player.inventory)
                {
                    if (item.type == ItemType)
                    {
                        int removed = Math.Min(item.stack, leftToRemove);
                        item.stack -= removed;
                        leftToRemove -= removed;
                        if (item.stack <= 0)
                            item.SetDefaults();
                        if (leftToRemove <= 0)
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
