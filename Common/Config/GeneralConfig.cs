using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SCPMod.Common.Config
{
    public class GeneralConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("SCP173")]
        [Label("Invincible")]
        [Tooltip("Should 173 be invincible?")]
        [DefaultValue(true)]
        public bool invincible173;

        [Label("Damaging pickaxe power")]
        [Tooltip("Pickaxe power needed to hurt 173")]
        [DefaultValue(110)]
        [Range(0, int.MaxValue)]
        public int pickToDamage173;

        [Label("Moves in darkness")]
        [Tooltip("Can 173 move in the pitch black?")]
        [DefaultValue(true)]
        public bool moveInDarkness173;
    }
}
