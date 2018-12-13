﻿﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DBZMOD.Buffs
{
    public class SSJ3Buff : TransBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Super Saiyan 3");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            DamageMulti = 4f;
            SpeedMulti = 4f;
            KiDrainBuffMulti = 2.1f;
            KiDrainRate = 6;
            KiDrainRateWithMastery = 4;
            Description.SetDefault(AssembleTransBuffDescription() + "\n(Life drains when below 30% Max Ki)");
        }
        public override void Update(Player player, ref int buffIndex)
        {
            MyPlayer modPlayer = MyPlayer.ModPlayer(player);
            bool isMastered = modPlayer.MasteryLevel3 >= 1f;

            KiDrainRate = isMastered ? KiDrainRate : KiDrainRateWithMastery;
            float kiQuotient = (float)modPlayer.GetKi() / modPlayer.OverallKiMax();
            if (kiQuotient <= 0.3f)
            {
                HealthDrainRate = isMastered ? 10 : 20;
            } else
            {
                HealthDrainRate = 0;
            }
            
            MasteryTimer++;
            if (!(MyPlayer.ModPlayer(player).playerTrait == "Prodigy") && MasteryTimer >= 300 && MyPlayer.ModPlayer(player).MasteryMax3 <= 1)
            {
                MyPlayer.ModPlayer(player).MasteryLevel3 += 0.01f;
                MasteryTimer = 0;
            }
            else if (MyPlayer.ModPlayer(player).playerTrait == "Prodigy" && MasteryTimer >= 150 && MyPlayer.ModPlayer(player).MasteryMax3 <= 1)
            {
                MyPlayer.ModPlayer(player).MasteryLevel3 += 0.01f;
                MasteryTimer = 0;
            }
            base.Update(player, ref buffIndex);
        }
    }
}

