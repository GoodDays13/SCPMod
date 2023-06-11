using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SCPMod.Common.Config
{
    public class GeneralConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("SCP173")]
        [DefaultValue(true)]
        public bool invincible173;

        [DefaultValue(110)]
        [Range(0, int.MaxValue)]
        public int pickToDamage173;

        [DefaultValue(true)]
        public bool moveInDarkness173;
    }
}
