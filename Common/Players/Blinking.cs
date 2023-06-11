﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using SCPMod.Common.Config;
using SCPMod.Common.Systems;
using SCPMod.Content.NPCs;
using static Terraria.GameContent.Events.ScreenDarkness;

namespace SCPMod.Common.Players
{
    public class Blinking : ModPlayer
    {
        public bool blinking => blink > 0;
        //public int facing { get; private set; }
        private int blink;
        private int blinkTimer;
        private int lastBlinkTimerSet;
        private void SetBlinkTimer(int time) { blinkTimer = time; lastBlinkTimerSet = blinkTimer; }
        private readonly int timeToBlinkMin = 360;
        private readonly int timeToBlinkMax = 600;
        private int TimeToBlink => Main.rand.Next(timeToBlinkMin, timeToBlinkMax + 1);

        public override void OnRespawn()
        {
            if (Main.player[Main.myPlayer] != Player)
                return;
            SetBlinkTimer(TimeToBlink);
        }

        public override void OnEnterWorld()
        {
            if (Main.player[Main.myPlayer] != Player)
                return;
            frontColor = Color.Black;
            SetBlinkTimer(TimeToBlink);
        }

        public override void PostUpdate()
        {
            if (Main.player[Main.myPlayer] != Player)
                return;
            if (NPC.AnyNPCs(ModContent.NPCType<SCP173>()))
                blinkTimer--;
            else {
                SetBlinkTimer(TimeToBlink);
                lastBlinkTimerSet += 60;
                return;
            }

            if (ModContent.GetInstance<ClientConfig>().blinkWarning)
                switch (blinkTimer - 4)
                {
                    case 120:
                    case 60:
                    case 0:
                        SoundEngine.PlaySound(SoundID.MenuTick with { Pitch = -0.5f, Volume = Main.soundVolume });
                        break;
                }

            if (blinkTimer == 1)
                Player.eyeHelper.BlinkBecausePlayerGotHurt();
            if (blinkTimer == 0)
                blink = 15;
            if (blink <= 1 && KeybindSystem.BlinkKeybind.Current)
                blink = 2;
            if (Player.dead)
                blink = 0;

            if (blinking)
            {
                SetBlinkTimer(TimeToBlink);
                blink--;

                screenObstruction = 1;
            }
            else if (blinkTimer < 5)
                screenObstruction = 1f - 0.2f * blinkTimer;
            else if (lastBlinkTimerSet - blinkTimer <= 5)
                screenObstruction = 1f - 0.2f * (lastBlinkTimerSet - blinkTimer);
        }
    }
}
