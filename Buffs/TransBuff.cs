﻿﻿using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using DBZMOD;
using Terraria;
using System;
using DBZMOD.Items.Accessories;
using Util;

namespace DBZMOD
{
    public abstract class TransBuff : ModBuff
    {
        public const int KI_DRAIN_TIMER_MAX = 3;
        public float DamageMulti;
        public float SpeedMulti;
        public float KaioLightValue;
        public float KiDrainBuffMulti;
        public float SSJLightValue;
        public int HealthDrainRate;
        public int OverallHealthDrainRate;
        public int KiDrainRate;
        public int KiDrainRateWithMastery;
        private int KiDrainTimer;
        private int KiDrainAddTimer;
        public bool RealismModeOn;
        public int MasteryTimer;
        
        public override void Update(Player player, ref int buffIndex)
        {
            MyPlayer modPlayer = MyPlayer.ModPlayer(player);

            KiDrainAdd(player);
            if(Transformations.IsKaioken(player))
            {
                Lighting.AddLight(player.Center, KaioLightValue, 0f, 0f);
            }

            // only neuter the life regen if this is a draining buff.
            if (HealthDrainRate > 0)
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegenTime = 0;

                // only apply the kaio crystal benefit if this is kaioken
                bool isKaioCrystalEquipped = player.IsAccessoryEquipped("Kaio Crystal");
                float drainMult = ((Transformations.IsKaioken(player) || Transformations.IsSSJ1Kaioken(player)) && isKaioCrystalEquipped ? 0.5f : 1f);

                // recalculate the final health drain rate and reduce regen by that amount
                OverallHealthDrainRate = (int)Math.Ceiling((float)HealthDrainRate * drainMult);
                player.lifeRegen -= OverallHealthDrainRate;
            }
            
            // if the player is in any ki-draining state, handles ki drain and power down when ki is depleted
            if (Transformations.IsSSJ(player) || Transformations.IsLSSJ(player) || Transformations.IsSSJ1Kaioken(player) || Transformations.IsAscended(player))
            {
                // player ran out of ki, so make sure they fall out of any forms they might be in.
                if (modPlayer.IsKiDepleted())
                {
                    // Main.NewText(string.Format("Player is out of ki??! {0} has {1} of {2} ki", player.whoAmI, modPlayer.GetKi(), modPlayer.OverallKiMax()));
                    Transformations.EndTransformations(player, true, false);
                }
                else
                {
                    // player still has some ki, perform drain routine
                    KiDrainTimer++;
                    if (KiDrainTimer >= KI_DRAIN_TIMER_MAX)
                    {
                        modPlayer.AddKi((KiDrainRate + modPlayer.KiDrainAddition) * -1);
                        KiDrainTimer = 0;
                    }
                    KiDrainAddTimer++;
                    if (KiDrainAddTimer > 600)
                    {
                        modPlayer.KiDrainAddition += 1;
                        KiDrainAddTimer = 0;
                    }
                    Lighting.AddLight(player.Center, 1f, 1f, 0f);
                }
            } else
            {
                // the player isn't in a ki draining state anymore, reset KiDrainAddition
                modPlayer.KiDrainAddition = 0;                
            }

            //DebugUtil.Log(string.Format("Before: Player moveSpeed {0} maxRunSpeed {1} runAcceleration {2} bonusSpeedMultiplier {3} speedMult {4}", player.moveSpeed, player.maxRunSpeed, player.runAcceleration, modPlayer.bonusSpeedMultiplier, SpeedMulti));
            player.moveSpeed *= 1f + (SpeedMulti * modPlayer.bonusSpeedMultiplier);
            player.maxRunSpeed *= 1f + (SpeedMulti * modPlayer.bonusSpeedMultiplier);
            player.runAcceleration *= 1f + (SpeedMulti * modPlayer.bonusSpeedMultiplier);
            //DebugUtil.Log(string.Format("After: Player moveSpeed {0} maxRunSpeed {1} runAcceleration {2} bonusSpeedMultiplier {3} speedMult {4}", player.moveSpeed, player.maxRunSpeed, player.runAcceleration, modPlayer.bonusSpeedMultiplier, SpeedMulti));

            // set player damage  mults
            player.meleeDamage *= GetHalvedDamageBonus();
            player.rangedDamage *= GetHalvedDamageBonus();
            player.magicDamage *= GetHalvedDamageBonus();
            player.minionDamage *= GetHalvedDamageBonus();
            player.thrownDamage *= GetHalvedDamageBonus();
            modPlayer.KiDamage *= DamageMulti;

            // cross mod support stuff
            if (DBZMOD.instance.thoriumLoaded)
            {
                ThoriumEffects(player);
            }
            if (DBZMOD.instance.tremorLoaded)
            {
                TremorEffects(player);
            }
            if (DBZMOD.instance.enigmaLoaded)
            {
                EnigmaEffects(player);
            }
            if (DBZMOD.instance.battlerodsLoaded)
            {
                BattleRodEffects(player);
            }
            if (DBZMOD.instance.expandedSentriesLoaded)
            {
                ExpandedSentriesEffects(player);
            }
        }

        public float GetHalvedDamageBonus()
        {
            return 1 + ((DamageMulti - 1) * 0.5f);
        }

        public void ThoriumEffects(Player player)
        {
            player.GetModPlayer<ThoriumMod.ThoriumPlayer>(ModLoader.GetMod("ThoriumMod")).symphonicDamage *= GetHalvedDamageBonus();
            player.GetModPlayer<ThoriumMod.ThoriumPlayer>(ModLoader.GetMod("ThoriumMod")).radiantBoost *= GetHalvedDamageBonus();
        }

        public void TremorEffects(Player player)
        {
            player.GetModPlayer<Tremor.MPlayer>(ModLoader.GetMod("Tremor")).alchemicalDamage *= GetHalvedDamageBonus();
        }

        public void EnigmaEffects(Player player)
        {
            player.GetModPlayer<Laugicality.LaugicalityPlayer>(ModLoader.GetMod("Laugicality")).mysticDamage *= GetHalvedDamageBonus();
        }

        public void BattleRodEffects(Player player)
        {
            player.GetModPlayer<UnuBattleRods.FishPlayer>(ModLoader.GetMod("UnuBattleRods")).bobberDamage *= GetHalvedDamageBonus();
        }

        public void ExpandedSentriesEffects(Player player)
        {
            player.GetModPlayer<ExpandedSentries.ESPlayer>(ModLoader.GetMod("ExpandedSentries")).sentryDamage *= GetHalvedDamageBonus();
        }

        private void KiDrainAdd(Player player)
        {
            MyPlayer.ModPlayer(player).KiDrainMulti = KiDrainBuffMulti;
        }

        public string GetPercentForDisplay(string currentDisplayString, string text, int percent)
        {
            if (percent == 0)
                return currentDisplayString;
            return string.Format("{0}{1} {2}{3}%", currentDisplayString, text, percent > 0 ? "+" : string.Empty, percent);
        }

        public string AssembleTransBuffDescription()
        {
            int percentDamageMult = (int)Math.Ceiling(DamageMulti * 100f) - 100;
            int percentSpeedMult = (int)Math.Ceiling(SpeedMulti * 100f) - 100;
            int kiDrainPerSecond = (60 / KI_DRAIN_TIMER_MAX) * KiDrainRate;
            int kiDrainPerSecondWithMastery = (60 / KI_DRAIN_TIMER_MAX) * KiDrainRateWithMastery;
            int percentKiDrainMulti = (int)Math.Ceiling(KiDrainBuffMulti * 100f) - 100;
            string displayString = string.Empty;
            displayString = GetPercentForDisplay(displayString, "Damage", percentDamageMult);
            displayString = GetPercentForDisplay(displayString, " Speed", percentSpeedMult);
            displayString = GetPercentForDisplay(displayString, "\nKi Costs", percentKiDrainMulti);
            if (kiDrainPerSecond > 0)
            {
                displayString = string.Format("{0}\nKi Drain: {1}/s", displayString, kiDrainPerSecond);
                if (kiDrainPerSecondWithMastery > 0)
                {
                    displayString = string.Format("{0}, {1}/s when mastered", displayString, kiDrainPerSecondWithMastery);
                }
            }
            if (HealthDrainRate > 0)
            {
                displayString = string.Format("{0}\nLife Drain: -{1}/s.", displayString, HealthDrainRate / 2);
            }
            return displayString;
        }
    }
}

