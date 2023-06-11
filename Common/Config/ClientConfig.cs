using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SCPMod.Common.Config
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Sounds")]
        [Label("Stinger sounds")]
        [Tooltip("The sound that plays when 173 gets close to you.")]
        [DefaultValue(true)]
        public bool horrorSounds;

        [Label("Stone drag sounds")]
        [Tooltip("The sound 173 makes when moving around. (can be buggy)")]
        [DefaultValue(false)]
        public bool stoneDragSounds;

        [Label("Blink warning sounds")]
        [Tooltip("A sound that plays when you blink and two times before.")]
        [DefaultValue(true)]
        public bool blinkWarning;
    }
}
