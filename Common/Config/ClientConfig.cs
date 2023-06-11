using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SCPMod.Common.Config
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Sounds")]
        [DefaultValue(true)]
        public bool horrorSounds;

        [DefaultValue(false)]
        public bool stoneDragSounds;

        [DefaultValue(true)]
        public bool blinkWarning;
    }
}
