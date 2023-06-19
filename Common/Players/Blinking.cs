using Microsoft.Xna.Framework;
using SCPMod.Common.Config;
using SCPMod.Common.Systems;
using SCPMod.Content.NPCs;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Events.ScreenDarkness;

namespace SCPMod.Common.Players
{
    public class Blinking : ModPlayer
    {
        public bool IsBlinking => blink > 0;
        private int blink;
        private int BlinkTimer;
        private int lastBlinkTimerSet;
        private bool ManualBlink;
        private readonly int timeToBlinkMin = 360;
        private readonly int timeToBlinkMax = 600;
        private int DefaultBlinkTime => Main.rand.Next(timeToBlinkMin, timeToBlinkMax + 1);

        private int GetLowestTimer()
        {
            int LowestTimer = int.MaxValue;
            foreach (Player p in Main.player)
            {
                if (p.active && p.whoAmI != Player.whoAmI && !p.DeadOrGhost && p.GetModPlayer<Blinking>().BlinkTimer < LowestTimer)
                    LowestTimer = p.GetModPlayer<Blinking>().BlinkTimer;
            }
            return LowestTimer == int.MaxValue ? 0 : LowestTimer;
        }

        private void SetBlinkTimer()
        {
            if (Main.myPlayer != Player.whoAmI)
                return;
            if (Main.netMode == NetmodeID.SinglePlayer || ModContent.GetInstance<GeneralConfig>().SyncBlinks == "None")
                BlinkTimer = DefaultBlinkTime;
            else if (ModContent.GetInstance<GeneralConfig>().SyncBlinks == "Fully")
            {
                int LowestTimer = GetLowestTimer();
                if (LowestTimer > 0)
                    BlinkTimer = LowestTimer;
                else
                    BlinkTimer = DefaultBlinkTime;
            }
            else //if (ModContent.GetInstance<GeneralConfig>().SyncBlinks == "Timeframe")
            {
                int targetTime = 0;
                foreach (Player p in Main.player)
                {
                    if (p.active && p != Player)
                    {
                        targetTime = p.GetModPlayer<Blinking>().BlinkTimer;
                        break;
                    }
                }
                if (targetTime <= 0)
                {
                    BlinkTimer = DefaultBlinkTime;
                }
                else
                {
                    int time = DefaultBlinkTime;
                    int round = (int)(ModContent.GetInstance<GeneralConfig>().TimeframeInSeconds * 60);
                    int modDiff = time % round - targetTime % round;
                    if (modDiff > round / 2)
                        modDiff -= round;
                    else if (modDiff < -round / 2)
                        modDiff += round;
                    time -= modDiff; // make mod same as target
                    BlinkTimer = time;
                }
            }
            lastBlinkTimerSet = BlinkTimer;

            SyncBlinkTimer();
        }

        public override void OnRespawn()
        {
            SetBlinkTimer();
        }

        public override void OnEnterWorld()
        {
            frontColor = Color.Black;
            SetBlinkTimer();
        }

        public override void PostUpdate()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<SCP173>()))
            {
                BlinkTimer = 0;
                return;
            }

            if (!ManualBlink && Main.myPlayer == Player.whoAmI && ModContent.GetInstance<ClientConfig>().BlinkWarning)
                switch (BlinkTimer - 4)
                {
                    case 120:
                    case 60:
                    case 0:
                        SoundEngine.PlaySound(SoundID.MenuTick with
                        {
                            Pitch = -0.5f,
                            Volume = Main.soundVolume + ModContent.GetInstance<ClientConfig>().BlinkWarningBoost / 100f
                        });
                        break;
                }

            if (BlinkTimer == 1)
                Player.eyeHelper.BlinkBecausePlayerGotHurt();
            if (BlinkTimer == 0)
                blink = 15;
            if (blink <= 1 && ManualBlink)
                blink = 2;
            if (Player.dead)
                blink = 0;

#if DEBUG
            if (BlinkTimer % 60 == 0)
                Print(BlinkTimer / 60 + " seconds for " + Player.name);
#endif

            if (IsBlinking)
            {
                blink--;
                if (blink == 5 && IsFirstPlayer())
                {
                    SetBlinkTimer();
                }
                else if (blink == 0 && !IsFirstPlayer())
                {
                    SetBlinkTimer();
                }
            }
            if (Main.myPlayer == Player.whoAmI)
            {
                if (IsBlinking)
                    screenObstruction = 1;
                if (BlinkTimer < 5)
                    screenObstruction = 1f - 0.2f * BlinkTimer;
                else if (lastBlinkTimerSet - BlinkTimer <= 5)
                    screenObstruction = 1f - 0.2f * (lastBlinkTimerSet - BlinkTimer);
            }

            BlinkTimer--;
        }

        private bool IsFirstPlayer()
        {
            if (ModContent.GetInstance<GeneralConfig>().SyncBlinks != "Fully")
                return false;
            foreach (Player p in Main.player)
            {
                if (p.active)
                {
                    return p.whoAmI == Player.whoAmI;
                }
            }
            return false;
        }

        public override void UpdateDead()
        {
            if (Main.myPlayer == Player.whoAmI)
                screenObstruction = 0;
            
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.BlinkKeybind.JustPressed)
                ManualBlink = true;
            else if (KeybindSystem.BlinkKeybind.JustReleased)
                ManualBlink = false;
            else
                return;

            SyncManualBlink();
        }

#if DEBUG
        public static void Print(string text)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                Console.WriteLine("Server: " + text);
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Console.WriteLine("Client: " + text);
            }
        } 
#endif


        // NETWORKING

        public void SyncManualBlink()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)SCPMod.MessageType.ManualBlink);
            packet.Write((byte)Player.whoAmI);
            packet.Write(ManualBlink);
            packet.Send(-1, Player.whoAmI);
        }

        public void SyncBlinkTimer()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)SCPMod.MessageType.BlinkTimer);
            packet.Write((byte)Player.whoAmI);
            packet.Write(BlinkTimer);
            packet.Send(-1, Player.whoAmI);
        }

        public void ReceiveManualBlinkSync(BinaryReader reader)
        {
            ManualBlink = reader.ReadByte() == 1;
        }

        public void ReceiveBlinkTimerSync(BinaryReader reader)
        {
            BlinkTimer = reader.ReadInt32();
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer = false)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)SCPMod.MessageType.PlayerSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)BlinkTimer);
            packet.Write(ManualBlink);
            packet.Send(toWho, fromWho);
        }

        public void RecieveSyncPlayer(BinaryReader reader)
        {
            BlinkTimer = reader.ReadByte();
            ManualBlink = reader.ReadByte() == 1;
        }

        //public override void CopyClientState(ModPlayer targetCopy)
        //{
        //    Blinking clone = (Blinking)targetCopy;
        //    clone.ManualBlink = ManualBlink;
        //    clone.BlinkTimer = BlinkTimer;
        //}

        //public override void SendClientChanges(ModPlayer clientPlayer)
        //{
        //    Blinking clone = (Blinking)clientPlayer;

        //    if (ManualBlink != clone.ManualBlink || BlinkTimer != clone.BlinkTimer)
        //        SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        //}
    }
}
