using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SCPMod.Common.Players;
using Terraria.Audio;
using Terraria.ID;
using SCPMod.Common.Config;
using SCPMod.Common.Systems;

namespace SCPMod.Content.NPCs
{
    public class SCP173 : ModNPC
    {
        private readonly float speed = 32;

        private SoundStyle stoneDrag = new SoundStyle($"SCPMod/Assets/173/StoneDrag") with
        {
            Volume = Main.soundVolume,
            MaxInstances = 1
        };

        private ref float Darkness => ref NPC.ai[0];
        private ref float Visible => ref NPC.ai[1];

        private static bool SolidTile(float x, float y) => SolidTile(PointFromCords(x, y));
        private static bool SolidTile(Point point) => Main.tile[point].HasTile &&
                        Main.tileSolid[Main.tile[point].TileType];
        private static Point PointFromCords(float x, float y) => new((int)x / 16, (int)y / 16);

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NoMultiplayerSmoothingByType[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 18; // note, visually 26
            NPC.height = 48;
            NPC.damage = 10000;
            NPC.lifeMax = 1000;
            NPC.HitSound = SoundID.Tink;
            //NPC.DeathSound = SoundID.Tink;
            NPC.value = 10000;
            NPC.knockBackResist = 1;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.chaseable = false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnTimers.timers[173] == 0 && !NPC.AnyNPCs(Type) ? 10000 : 0;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return CheckSeen();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.KillMe(Terraria.DataStructures.PlayerDeathReason.ByCustomReason($"{target.name} had their neck snapped."), 1000, 0);
            SoundEngine.PlaySound(new SoundStyle($"SCPMod/Assets/173/NeckSnap") with { 
                Volume = Main.soundVolume,
                Variants = new int[] { 1, 2, 3 }
            }, target.position);
        }

        public override void AI()
        {
            MoveDown();
            float temp = Visible;

            if (!Main.dedServ && ModContent.GetInstance<GeneralConfig>().MoveInDarkness173)
                Darkness = Lighting.GetColor(PointFromCords(NPC.position.X, NPC.position.Y)) == Color.Black ? 1 : 0; // TODO check all blocks it's in
            else
                Darkness = 0;

            Visible = CheckSeen() ? 1 : 0;

            if (Main.netMode != NetmodeID.Server)
            {
                if (ModContent.GetInstance<ClientConfig>().HorrorSounds &&
                    Visible == 1 && temp == 0 && !Main.player[Main.myPlayer].DeadOrGhost && 
                    Main.player[Main.myPlayer].position.Distance(NPC.position) - 
                        NPC.width/2 - Player.defaultWidth/2 < speed * 2)
                    SoundEngine.PlaySound(new SoundStyle($"SCPMod/Assets/Horror/Horror") with
                    {
                        Volume = Main.soundVolume,
                        Variants = new int[] { 0, 1, 2, 3, 4, 5, 9, 10 }
                    });
            }
            
            if (Visible == 1)
            {
                NPC.aiStyle = -1;
                NPC.velocity.X = 0;
                NPC.velocity.Y = 0;
                if (Main.rand.NextBool(800))
                    SoundEngine.PlaySound(new SoundStyle($"SCPMod/Assets/173/Rattle") with
                    {
                        Volume = Main.soundVolume,
                        Variants = new int[] {1, 2, 3}
                    }, NPC.position);
                if (!false)
                {
                    ActiveSound dragSound = SoundEngine.FindActiveSound(stoneDrag);
                    dragSound?.Stop();
                }
            }
            else // not visible, move
            {
                NPC.TargetClosest();
                NPC.velocity.X = NPC.direction * speed;
                NPC.velocity.Y = 0;
                if (Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) < speed) // if close enough to center on player, center instead
                {
                    NPC.velocity.X = Main.player[NPC.target].Center.X - NPC.Center.X;

                    if (NPC.Bottom.Y - Main.player[NPC.target].Bottom.Y > 0 &&
                        NPC.Bottom.Y - Main.player[NPC.target].Bottom.Y < 16 * 6)
                        TryJump(NPC.Center.Y - Main.player[NPC.target].Center.Y);
                }

                Point bottomBlock;
                Point topBlock;
                if (NPC.direction > 0)
                {
                    bottomBlock = PointFromCords(NPC.BottomRight.X, NPC.BottomRight.Y - 1); // TODO see if i should move x one closer
                    topBlock = PointFromCords(NPC.TopRight.X, NPC.TopRight.Y);
                }
                else
                {
                    bottomBlock = PointFromCords(NPC.BottomLeft.X, NPC.BottomLeft.Y - 1);
                    topBlock = PointFromCords(NPC.TopLeft.X, NPC.TopLeft.Y);
                }

                bool jumped = false;
                for (int i = 1; i <= Math.Abs((int)NPC.velocity.X) / 16; i++) { // check where 173 will go for collisions
                    topBlock.X += NPC.direction;
                    bottomBlock.X += NPC.direction;
                    if (CheckCollision(topBlock, bottomBlock))
                    {
                        if (jumped)
                        {
                            // move up against next block
                            bool right = NPC.direction > 0;
                            NPC.velocity.X = (bottomBlock.X * 16 + (right ? 0 : 1)) - (right ? NPC.Right.X : NPC.Left.X);
                            break;
                        }
                        else if (!Jump(topBlock, bottomBlock)) // try to jump over collision
                        {
                            // if failed, just move up against block
                            bool right = NPC.direction > 0;
                            NPC.velocity.X = (bottomBlock.X * 16 + (right ? 0 : 1)) - (right ? NPC.Right.X : NPC.Left.X);
                        }
                        else // successful jump
                        {
                            // reset block X and set new block Y to check post jump
                            bottomBlock.X -= NPC.direction * i;
                            bottomBlock.Y += (int)NPC.velocity.Y / 16;
                            topBlock.X -= NPC.direction * i;
                            topBlock.Y += (int)NPC.velocity.Y / 16;
                            jumped = true;
                            i = 0;
                        }
                    }
                }

                if (Main.netMode == NetmodeID.Server || !ModContent.GetInstance<ClientConfig>().StoneDragSounds)
                    return;

                ActiveSound dragSound = SoundEngine.FindActiveSound(stoneDrag);
                if (NPC.velocity.X >= 1 && (dragSound == null || !dragSound.IsPlaying))
                    SoundEngine.PlaySound(stoneDrag, NPC.position);
                else if (NPC.velocity.X < 1 && dragSound != null && dragSound.IsPlaying)
                    dragSound.Stop();
            }
            //if (SolidTile(NPC.BottomRight.X, NPC.BottomRight.Y - 16) ||
            //    SolidTile(NPC.BottomLeft.X, NPC.BottomLeft.Y - 16))
            //    NPC.position.Y -= speed;
        }

        private void MoveDown()
        {
            Point leftPoint = PointFromCords(NPC.BottomLeft.X + 1, NPC.BottomLeft.Y);
            Point rightPoint = PointFromCords(NPC.BottomRight.X - 1, NPC.BottomRight.Y);
            if (!(SolidTile(leftPoint) || SolidTile(rightPoint)))
                for (int i = 1; i < 64; i++)
                {
                    leftPoint.Y++;
                    rightPoint.Y++;
                    if (SolidTile(leftPoint))
                    {
                        NPC.position.Y = leftPoint.Y * 16 - NPC.height;
                        break;
                    }
                    if (SolidTile(rightPoint))
                    {
                        NPC.position.Y = leftPoint.Y * 16 - NPC.height;
                        break;
                    }
                }
            else
                NPC.position.Y = leftPoint.Y * 16 - NPC.height;
        }

        /// <summary>
        /// Tries to jump over something 173 will collide with
        /// </summary>
        /// <param name="topPoint">Top of 173 where he will collide</param>
        /// <param name="bottomPoint">Bottom of 173 where he will collideparam>
        /// <returns>Whether or not it was successful</returns>
        private bool Jump(Point topPoint, Point bottomPoint)
        {
            for (int i = 1; i <= 6; i++)
            {
                topPoint.Y--;
                bottomPoint.Y--;
                if (SolidTile(NPC.position.X, NPC.Top.Y - 16 * i))
                    break;
                if (!CheckCollision(topPoint, bottomPoint)) {
                    NPC.velocity.Y = -16 * i;
                    return true;
                }
            }
            return false;
        }

        private void TryJump(float distance)
        {
            Point check = PointFromCords(NPC.Center.X, NPC.Center.Y);
            for (int i = 1; i <= distance / 16; i++)
            {
                check.Y--;
                if (SolidTile(check))
                    return;
            }
            NPC.velocity.Y -= distance;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return Visible != 1;
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            if (ModContent.GetInstance<GeneralConfig>().Invincible173)
                return false;
            if (item.pick >= ModContent.GetInstance<GeneralConfig>().PickToDamage173)
                return null;
            return false;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;

        public override void OnKill()
        {
            SpawnTimers.timers[173] = 6300 * 1;
        }

        private bool CheckSeen()
        {
            //if (Lighting.GetColor(PointFromCords(NPC.position.X, NPC.position.Y)) == Color.Black)
            //    return false; doesn't work on server? perhaps submit git problem? TODO

            var players = new List<Player>();
            foreach (Player p in Main.player)
            {
                if (!p.active)
                    break;
                players.Add(p);
            }

            bool allPlayersDead = true;
            foreach (var player in players)
            {
                if (player.DeadOrGhost)
                    continue;
                allPlayersDead = false;
                if (CheckSeen(player))
                    return true;
            }
            return allPlayersDead;
        }

        private bool CheckSeen(Player player)
        {
            if (Darkness == 1)
                return false;
            if (player.DeadOrGhost)
                return true;
            if (!CheckInRange(player))
                return false;
            if (player.GetModPlayer<Blinking>().IsBlinking)
                return false;
            return true;
        }

        private bool CheckInRange(Player player)
        {
            if (NPC.Left.X - speed > player.Center.X + 1920 / 2)
                return false;
            if (NPC.Right.X + speed < player.Center.X - 1920 / 2)
                return false;
            if (NPC.Bottom.Y < player.Center.Y - 1080 / 2)
                return false;
            if (NPC.Top.Y > player.Center.Y + 1080 / 2)
                return false;
            return true;
        }

        /// <summary>
        /// Checks for a collision within a vertical collumn of blocks
        /// </summary>
        /// <param name="top">Top block of the collumn to check</param>
        /// <param name="bottom">Bottom block of the collumn to check</param>
        /// <returns>If there were any solid tiles</returns>
        private static bool CheckCollision(Point top, Point bottom)
        {
            for (int i = bottom.Y; i >= top.Y; i--)
            {
                if (SolidTile(new Point(top.X, i)))
                {
                    return true;
                }
            }
            return false;
        }

        //private bool CheckFacing(Player player)
        //{
        //    return true;
        //    int facing = player.GetModPlayer<Blinking>().facing;
        //    if (facing < 0)
        //    {
        //        return NPC.Left.X < player.Right.X;
        //    }
        //    else
        //    {
        //        return player.Left.X < NPC.Right.X;
        //    }
        //}
    }
}
