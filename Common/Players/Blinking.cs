using Microsoft.Xna.Framework;
using SCPMod.Common.Config;
using SCPMod.Common.Systems;
using SCPMod.Content.NPCs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Events.ScreenDarkness;

namespace SCPMod.Common.Players
{
    public class Blinking : ModPlayer
    {
        public bool IsBlinking => blink > 0;
        //public int facing { get; private set; }
        private int blink;
        public int BlinkTimer { get; set; }
        private int lastBlinkTimerSet;
        private void SetBlinkTimer(int time) { BlinkTimer = time; lastBlinkTimerSet = BlinkTimer; }
        private void SetBlinkTimer() { SetBlinkTimer(Main.rand.Next(timeToBlinkMin, timeToBlinkMax + 1)); }
        public void RoundBlinkTimer(int frames) { BlinkTimer -= BlinkTimer % frames; }
        private readonly int timeToBlinkMin = 360;
        private readonly int timeToBlinkMax = 600;

        public override void OnRespawn()
        {
            if (Main.player[Main.myPlayer] != Player)
                return;
            SetBlinkTimer();
        }

        public override void OnEnterWorld()
        {
            if (Main.player[Main.myPlayer] != Player)
                return;
            frontColor = Color.Black;
            SetBlinkTimer();
        }

        public override void PostUpdate()
        {
            //if (Main.player[Main.myPlayer] != Player)
            //    return;
            if (NPC.AnyNPCs(ModContent.NPCType<SCP173>()))
                BlinkTimer--;
            else
            {
                SetBlinkTimer();
                lastBlinkTimerSet += 60;
                return;
            }

            if (ModContent.GetInstance<ClientConfig>().BlinkWarning)
                switch (BlinkTimer - 4)
                {
                    case 120:
                    case 60:
                    case 0:
                        SoundEngine.PlaySound(SoundID.MenuTick with { Pitch = -0.5f, Volume = Main.soundVolume });
                        break;
                }

            if (BlinkTimer == 1)
                Player.eyeHelper.BlinkBecausePlayerGotHurt();
            if (BlinkTimer == 0)
                blink = 15;
            if (blink <= 1 && KeybindSystem.BlinkKeybind.Current)
                blink = 2;
            if (Player.dead)
                blink = 0;

            if (IsBlinking)
            {
                SetBlinkTimer();
                blink--;

                screenObstruction = 1;
            }
            else if (BlinkTimer < 5)
                screenObstruction = 1f - 0.2f * BlinkTimer;
            else if (lastBlinkTimerSet - BlinkTimer <= 5)
                screenObstruction = 1f - 0.2f * (lastBlinkTimerSet - BlinkTimer);
        }
    }
}
