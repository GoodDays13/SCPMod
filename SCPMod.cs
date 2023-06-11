using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace SCPMod
{
	public class SCPMod : Mod
	{
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