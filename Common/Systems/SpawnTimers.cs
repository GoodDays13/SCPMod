using System.Collections.Generic;
using Terraria.ModLoader;

namespace SCPMod.Common.Systems
{
    public class SpawnTimers : ModSystem
    {
        public static Dictionary<int, int> timers;

        public override void OnWorldLoad()
        {
            timers = new Dictionary<int, int>
            {
                [173] = 0
            };
        }

        public override void PreUpdateNPCs()
        {
            foreach (int key in timers.Keys)
                if (timers[key] > 0)
                    timers[key]--;
        }
    }
}
