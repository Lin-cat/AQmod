﻿using AQMod.Common.Utilities;
using AQMod.Items.Bait;
using AQMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public class AQProjectile : GlobalProjectile
    {
        public static class AIStyles
        {
            public const int BulletAI = 0;
            public const int ArrowAI = 1;
            public const int ThrownAI = 2;
            public const int BoomerangAI = 3;
            public const int VilethornAI = 4;
            public const int FallingStarAI = 5;
            public const int PowderAI = 6;
            public const int GrapplingHookAI = 7;
            public const int BounceAI = 8;
            public const int MagicMissileAI = 9;
            public const int FallingBlockAI = 10;
            public const int ShadowOrbPetAI = 11;
            public const int AquaScepterAI = 12;
            public const int HarpoonAI = 13;
            public const int GlowstickAI = 14;
            public const int FlailAI = 15;
            public const int ExplosiveAI = 16;
            public const int TombstoneAI = 17;
            public const int DemonSickleAI = 18;
            public const int SpearAI = 19;
            public const int DrillAI = 20;
            public const int HarpNotesAI = 21;
            public const int IceRodAI = 22;
            public const int FlamesAI = 23;
            public const int CrystalStormAI = 24;
            public const int BoulderAI = 25;
            public const int PetAI = 26;
        }

        public static class Sets
        {
            public static bool[] UntaggableProjectile { get; private set; }
            public static bool[] HeadMinion { get; private set; }
            public static bool[] UnaffectedByWind { get; private set; }

            internal static void LoadSets()
            {
                UntaggableProjectile = new bool[ProjectileLoader.ProjectileCount];
                UntaggableProjectile[ModContent.ProjectileType<CelesteTorusCollider>()] = true;

                HeadMinion = new bool[ProjectileLoader.ProjectileCount];
                HeadMinion[ModContent.ProjectileType<Projectiles.Summon.Monoxider>()] = true;

                UnaffectedByWind = new bool[ProjectileLoader.ProjectileCount];

                for (int i = 0; i < ProjectileLoader.ProjectileCount; i++)
                {
                    try
                    {
                        Projectile projectile = new Projectile();
                        projectile.SetDefaults(i);
                        if (projectile.aiStyle != AIStyles.BulletAI && projectile.aiStyle != AIStyles.ArrowAI && projectile.aiStyle != AIStyles.ThrownAI && 
                            projectile.aiStyle != AIStyles.GrapplingHookAI && projectile.aiStyle != AIStyles.DemonSickleAI && projectile.aiStyle != AIStyles.BoulderAI && 
                            projectile.aiStyle != AIStyles.BoomerangAI && projectile.aiStyle != AIStyles.ExplosiveAI && projectile.aiStyle != AIStyles.HarpNotesAI &&
                            projectile.aiStyle != AIStyles.BounceAI && projectile.aiStyle != AIStyles.CrystalStormAI)
                            UnaffectedByWind[i] = true;
                    }
                    catch
                    {
                    }
                }
            }

            internal static void UnloadSets()
            {
                UntaggableProjectile = null;
                HeadMinion = null;
                UnaffectedByWind = null;
            }
        }

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        /// <summary>
        /// When this flag is raised, no wind events should be applied to this Projectile
        /// </summary>
        public bool windStruck;
        public bool windStruckOld;

        public bool ShouldApplyWindMechanics(Projectile projectile)
        {
            return !windStruck && !Sets.UnaffectedByWind[projectile.type];
        }

        public void ApplyWindMechanics(Projectile projectile, Vector2 wind)
        {
            projectile.velocity += wind;
            windStruck = true;
        }

        public override bool PreAI(Projectile projectile)
        {
            windStruckOld = windStruck;
            windStruck = false;
            if (projectile.aiStyle == 61)
            {
                if (projectile.ai[1] > 0f && projectile.localAI[1] >= 0f) // on catch effects
                {
                    var aQPlayer = Main.player[projectile.owner].GetModPlayer<AQPlayer>();
                    if (aQPlayer.PopperType > 0)
                    {
                        var item = new Item();
                        item.SetDefaults(aQPlayer.PopperType);
                        ((PopperBaitItem)item.modItem).OnCatchEffect(Main.player[projectile.owner], aQPlayer, projectile, Framing.GetTileSafely(projectile.Center.ToTileCoordinates()));
                    }
                }
                else if (projectile.ai[0] <= 0f && projectile.wet && projectile.rotation != 0f) // When it enters the water
                {
                    var aQPlayer = Main.player[projectile.owner].GetModPlayer<AQPlayer>();
                    if (aQPlayer.PopperType > 0)
                    {
                        var item = new Item();
                        item.SetDefaults(aQPlayer.PopperType);
                        ((PopperBaitItem)item.modItem).PopperEffects(Main.player[projectile.owner], aQPlayer, projectile, Framing.GetTileSafely(projectile.Center.ToTileCoordinates()));
                    }
                }
            }
            return true;
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            if (projectile.aiStyle == 61)
            {
                if (projectile.ai[0] >= 1f && projectile.ai[1] > 0f && Main.myPlayer == projectile.owner)
                {
                    var plr = Main.player[projectile.owner];
                    var aQPlr = plr.GetModPlayer<AQPlayer>();
                    var item = new Item();
                    item.SetDefaults((int)projectile.ai[1]);
                    if (aQPlr.goldSeal && item.value > Item.sellPrice(gold: 1))
                    {
                        int sonarPotionIndex = plr.FindBuffIndex(BuffID.Sonar);
                        if (sonarPotionIndex != -1)
                        {
                            plr.buffTime[sonarPotionIndex] += 6000;
                        }
                        else
                        {
                            plr.AddBuff(BuffID.Sonar, 6000);
                        }
                    }
                    if (aQPlr.silverSeal)
                    {
                        int fishingPotionIndex = plr.FindBuffIndex(BuffID.Fishing);
                        if (fishingPotionIndex != -1)
                        {
                            plr.buffTime[fishingPotionIndex] += 600;
                        }
                        else
                        {
                            plr.AddBuff(BuffID.Fishing, 600);
                        }
                    }
                    switch (projectile.ai[1])
                    {
                        case ItemID.CorruptFishingCrate:
                        case ItemID.CrimsonFishingCrate:
                        case ItemID.DungeonFishingCrate:
                        case ItemID.FloatingIslandFishingCrate:
                        case ItemID.HallowedFishingCrate:
                        case ItemID.JungleFishingCrate:
                        case ItemID.GoldenCrate:
                        case ItemID.IronCrate:
                        case ItemID.WoodenCrate:
                        if (aQPlr.copperSeal)
                        {
                            int cratePotionIndex = plr.FindBuffIndex(BuffID.Crate);
                            if (cratePotionIndex != -1)
                            {
                                plr.buffTime[cratePotionIndex] += 1800;
                            }
                            else
                            {
                                plr.AddBuff(BuffID.Crate, 1800);
                            }
                        }
                        break;

                        default:
                        if (projectile.ai[1] > Main.maxItemTypes)
                        {
                            var modItem = ItemLoader.GetItem((int)projectile.ai[1]);
                            if (modItem.CanRightClick())
                                goto case ItemID.WoodenCrate;
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Counts how many projectiles are active of a given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int CountProjectiles(int type)
        {
            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == type)
                {
                    count++;
                }
            }
            return count;
        }

        public static void UpdateHeldProjDoVelocity(Player player, Vector2 rotatedRelativePoint, Projectile projectile)
        {
            float speed = 1f;
            var item = player.ItemInHand();
            if (item.shoot == projectile.type)
            {
                speed = item.shootSpeed * projectile.scale;
            }
            Vector2 newVelocity = (Main.MouseWorld - rotatedRelativePoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;
            if (projectile.velocity.X != newVelocity.X || projectile.velocity.Y != newVelocity.Y)
            {
                projectile.netUpdate = true;
            }
            projectile.velocity = newVelocity;
        }

        public static void UpdateHeldProj(Player player, Vector2 rotatedRelativePoint, float offsetAmount, Projectile projectile)
        {
            float velocityAngle = projectile.velocity.ToRotation();
            projectile.direction = (Math.Cos(velocityAngle) > 0.0).ToDirectionInt();
            float offset = offsetAmount * projectile.scale;
            projectile.position = rotatedRelativePoint - (projectile.Size * 0.5f) + (velocityAngle.ToRotationVector2() * offset);
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = velocityAngle + ((projectile.spriteDirection == -1).ToInt() * (float)Math.PI);
            player.ChangeDir(projectile.direction);
            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
            player.heldProj = projectile.whoAmI;
        }
    }
}