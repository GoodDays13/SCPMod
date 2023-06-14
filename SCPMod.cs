using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using System.IO;
using SCPMod.Common.Players;
using SCPMod.Common.Systems;

namespace SCPMod
{
	public class SCPMod : Mod
	{
        public enum MessageType : byte
        {
            PlayerSync,
            ManualBlink,
            BlinkTimer,
            BlinkSyncTimer
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType type = (MessageType)reader.ReadByte();
            switch (type)
            {
                case MessageType.PlayerSync:
                    int playerID = reader.ReadByte();
                    Blinking targetPlayer = Main.player[playerID].GetModPlayer<Blinking>();
                    targetPlayer.ReceiveBlinkTimerSync(reader);
                    break;
                case MessageType.ManualBlink:
                    playerID = reader.ReadByte();
                    targetPlayer = Main.player[playerID].GetModPlayer<Blinking>();
                    targetPlayer.ReceiveManualBlinkSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        // Forward the changes to the other clients
                        targetPlayer.SyncManualBlink();
                    }
                    break;
                case MessageType.BlinkTimer:
                    playerID = reader.ReadByte();
                    targetPlayer = Main.player[playerID].GetModPlayer<Blinking>();
                    targetPlayer.ReceiveBlinkTimerSync(reader);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        // Forward the changes to the other clients
                        targetPlayer.SyncBlinkTimer();
                    }
                    break;
                //case MessageType.BlinkSyncTimer:
                //    BlinkSync.RecieveSync();
                //    break;
                default:
                    Logger.WarnFormat("SCPMod: Unknown Message type: {0}", type);
                    break;
            }
        }
        //public override void Load()
        //{
        //    if (Main.netMode != NetmodeID.Server)
        //    {
        //        // First, you load in your shader file.
        //        // You'll have to do this regardless of what kind of shader it is,
        //        // and you'll have to do it for every shader file.
        //        // This example assumes you have both armour and screen shaders.

        //        Ref<Effect> filterRef = new Ref<Effect>();

        //        // To bind a screen shader, use this.
        //        // EffectPriority should be set to whatever you think is reasonable.   

        //        Filters.Scene["Blink"] = new Filter(new ScreenShaderData(filterRef, "PassName"), EffectPriority.Medium);
        //    }
        //}
    }
}