﻿using AQMod.Common.Graphics;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Fishing
{
    public class NimrodCloud : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.RainCloudRaining;

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.RainCloudRaining];
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.BobberWooden);
            drawOriginOffsetY = -8;
        }

        public Vector2 gotoPosition = new Vector2(0f, 0f);

        public override bool PreAI()
        {
            if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].fishingPole == 0 || Main.player[projectile.owner].CCed || Main.player[projectile.owner].noItems
               || Main.player[projectile.owner].pulley || Main.player[projectile.owner].dead || projectile.wet)
            {
                projectile.Kill();
            }

            if ((int)gotoPosition.X == -2f)
            {
                projectile.frame = 0;
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center) * 12f, 0.6f);
                projectile.timeLeft = 10;
                if ((projectile.Center - Main.player[projectile.owner].Center).Length() < 10f)
                {
                    projectile.Kill();
                }
                projectile.rotation += 0.1f;
                return false;
            }

            if ((int)gotoPosition.X == -1f)
            {
                projectile.frame = 1;
                projectile.velocity *= 0.9f;
                projectile.timeLeft = 20;
                if ((Main.player[projectile.owner].Center - projectile.Center).Length() > 3000f)
                {
                    projectile.ai[0] = 1f;
                    gotoPosition.X = -2f;
                    projectile.netUpdate = true;
                }
                if (Main.myPlayer == projectile.owner)
                {
                    if ((int)gotoPosition.Y != -20)
                    {
                        if (projectile.velocity.Length() < 1f)
                        {
                            gotoPosition.Y = -20;
                            SoundID.Item8.Play(projectile.Center);
                            Projectile.NewProjectile(projectile.Center, new Vector2(0f, 1f), ModContent.ProjectileType<NimrodElectric>(), projectile.damage, projectile.knockBack, projectile.owner);
                            Projectile.NewProjectile(projectile.Center + new Vector2(20f, 0f), new Vector2(1f, 1f), ModContent.ProjectileType<NimrodElectric>(), projectile.damage, projectile.knockBack, projectile.owner);
                            Projectile.NewProjectile(projectile.Center + new Vector2(-20f, 0f), new Vector2(-1f, 1f), ModContent.ProjectileType<NimrodElectric>(), projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }
                    else
                    {
                        bool shouldKill = true;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].bobber && Main.projectile[i].owner == projectile.owner && Main.projectile[i].type == ModContent.ProjectileType<NimrodElectric>())
                            {
                                shouldKill = false;
                            }
                        }
                        if (shouldKill)
                        {
                            projectile.ai[0] = 1f;
                            gotoPosition.X = -2f;
                            projectile.netUpdate = true;
                        }
                    }
                    if (Main.player[projectile.owner].controlUseItem)
                    {
                        projectile.ai[0] = 1f;
                        gotoPosition.X = -2f;
                        projectile.netUpdate = true;
                    }
                }
                projectile.rotation = 0f;
                return false;
            }
            projectile.timeLeft = 10;
            if (Main.myPlayer == projectile.owner && gotoPosition == new Vector2(0f, 0f))
            {
                gotoPosition = Main.MouseWorld + new Vector2(0f, -20f);
                projectile.netUpdate = true;
            }
            if ((projectile.Center - gotoPosition).Length() < 10f)
            {
                gotoPosition = new Vector2(-1f, -1f);
                projectile.velocity *= 0.1f;
                projectile.netUpdate = true;
            }
            else
            {
                projectile.velocity = Vector2.Normalize(gotoPosition - projectile.Center) * Main.player[projectile.owner].HeldItem.shootSpeed;
                projectile.rotation += 0.1f;
            }
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(gotoPosition.X);
            writer.Write(gotoPosition.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            gotoPosition.X = reader.ReadSingle();
            gotoPosition.Y = reader.ReadSingle();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            gotoPosition = new Vector2(-2f, -2f);
            return true;
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            var player = Main.player[projectile.owner];
            if (!projectile.bobber || player.inventory[player.selectedItem].holdStyle <= 0)
                return false;
            AQGraphics.Rendering.FishingLine_NoLighting(new Color(0, 172, 255, 200), player, projectile.position, projectile.width / 2, projectile.height, projectile.velocity, projectile.localAI[0], new Vector2(42f, 30f));
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var drawCoordinates = projectile.Center - Main.screenPosition;
            if (projectile.frame == 0)
            {
                Main.instance.LoadProjectile(ProjectileID.RainCloudMoving);
                var texture = Main.projectileTexture[ProjectileID.RainCloudMoving];
                var frame = texture.Frame(1, 4, 0, (int)Main.GameUpdateCount / 8 % 4);
                Main.spriteBatch.Draw(texture, drawCoordinates, frame, lightColor, projectile.rotation, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            }
            else
            {
                var texture = Main.projectileTexture[projectile.type];
                var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, (int)Main.GameUpdateCount / 8 % Main.projFrames[projectile.type]);
                Main.spriteBatch.Draw(texture, drawCoordinates, frame, lightColor, projectile.rotation, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}