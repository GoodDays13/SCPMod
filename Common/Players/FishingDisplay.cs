//using Microsoft.Xna.Framework;
//using System.Collections.Generic;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.GameContent.ItemDropRules;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace SCPMod.Common.Players
//{
//    public class FishingDisplay : ModPlayer
//    {
//        public bool displayActive;
//        public int lastFishingPower = -1;

//        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
//        {
//            lastFishingPower = attempt.fishingLevel;
//        }

//        //public override void GetFishingLevel(Item fishingRod, Item bait, ref float fishingLevel)
//        //{
//        //    //float tempLuck = Player.luck;
//        //    //while (tempLuck > 1)
//        //    //{
//        //    //    tempLuck--;

//        //    //    if (tempLuck < 0.0)
//        //    //    {
//        //    //        if ((double)Main.rand.NextFloat() < -(double)tempLuck)
//        //    //            fishingLevel = (int)((double)fishingLevel * (0.9 - (double)Main.rand.NextFloat() * 0.3));
//        //    //    }
//        //    //    else if ((double)Main.rand.NextFloat() < (double)tempLuck)
//        //    //        fishingLevel = (int)((double)fishingLevel * (1.1 + (double)Main.rand.NextFloat() * 0.3));
//        //    //}

//        //    //fishingLevel *= 1000000;
//        //}

//        public override void ResetEffects()
//        {
//            displayActive = false;
//        }

//        public override void UpdateEquips()
//        {
//            if (Player.accFishFinder)
//                displayActive = true;
//        }
//    }
//}