using SCPMod.Common.Config;
using SCPMod.Common.Players;
using SCPMod.Content.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SCPMod.Common.Systems
{
    public class BlinkSync : ModSystem
    {
        private int syncTimer;
        private int syncTime;
        private bool dontRun = false;
        private System system;
        private enum System
        {
            Timeframe,
            Fully
        }

        public override void OnWorldLoad()
        {
            if (ModContent.GetInstance<GeneralConfig>().SyncBlinks == "None" || Main.netMode == NetmodeID.SinglePlayer)
            {
                dontRun = true;
            }
            else
            {
                if (ModContent.GetInstance<GeneralConfig>().SyncBlinks == "Timeframe")
                    system = System.Timeframe;
                else
                    system = System.Fully;
                syncTime = (int)ModContent.GetInstance<GeneralConfig>().SecondsBetweenBlinkSync * 60;
                syncTimer = syncTime;
            }
        }

        public override void PostUpdatePlayers()
        {
            if (dontRun || !NPC.AnyNPCs(ModContent.NPCType<SCP173>()))
                return;

            if (syncTimer == 0)
            {
                if (system == System.Timeframe)
                    Timeframe(syncTime);
                else
                    Fully();
                syncTimer = syncTime;
            }
            syncTimer--;
        }

        private static void Timeframe(int frame)
        {
            foreach (Player p in Main.player)
            {
                if (p.active)
                    p.GetModPlayer<Blinking>().RoundBlinkTimer(frame);
            }
        }

        private static void Fully()
        {
            int LowestTimer = Main.player[0].GetModPlayer<Blinking>().BlinkTimer;
            foreach (Player p in Main.player)
            {
                if (p.active && p.GetModPlayer<Blinking>().BlinkTimer < LowestTimer)
                    LowestTimer = p.GetModPlayer<Blinking>().BlinkTimer;
            }
            foreach (Player p in Main.player)
            {
                if (p.active)
                    p.GetModPlayer<Blinking>().BlinkTimer = LowestTimer;
            }
        }
    }
}
