using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SCPMod.Common.Config
{
    public class GeneralConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("SCP173")]
        [DefaultValue(360)]
        [Range(120, 600)]
        public int Speed173;

        [DefaultValue(true)]
        public bool Invincible173;

        [DefaultValue(110)]
        [Range(0, int.MaxValue)]
        public int PickToDamage173;

        [DefaultValue(true)]
        public bool MoveInDarkness173;

        [DrawTicks]
        [OptionStrings(new string[] { "Fully", "Timeframe", "None" })]
        [DefaultValue("Timeframe")]
        public string SyncBlinks;

        [DefaultValue(1f)]
        [Range(0, 2f)]
        public float TimeframeInSeconds;
    }
}
