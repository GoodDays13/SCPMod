//using SCPMod.Common.Systems;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Terraria;
//using Terraria.GameContent;
//using Terraria.GameInput;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace SCPMod.Common.Players
//{
//    public class DamageResistPower : ModPlayer
//    {
//        public int cooldown = 0;
//        public int duration = 0;

//        public override void ProcessTriggers(TriggersSet triggersSet)
//        {
//            if (duration > 0)
//            {
//                if (--duration == 0)
//                    cooldown = 1800;
//            }
//            else if (cooldown > 0)
//            {
//                cooldown--;
//                if (cooldown % 60 == 0)
//                    Main.NewText($"Cooldown left: {cooldown / 60}");
//            }
//            else if (KeybindSystem.BlinkKeybind.JustPressed)
//            {
//                duration = 600;
//            }
//        }

//        public override void PreUpdate()
//        {
//            Player.endurance = 0.5f;
//        }
//    }
//}
