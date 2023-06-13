using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SCPMod.Common.Config
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Sounds")]
        [DefaultValue(true)]
        public bool HorrorSounds;

        [DefaultValue(false)]
        public bool StoneDragSounds;

        [DefaultValue(true)]
        public bool BlinkWarning;
    }
}
