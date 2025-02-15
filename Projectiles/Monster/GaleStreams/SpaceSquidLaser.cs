﻿using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Effects.Prims;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster.GaleStreams
{
    public class SpaceSquidLaser : ModProjectile
    {
        private PrimRenderer prim;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 360;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.coldDamage = true;

            projectile.GetGlobalProjectile<AQProjectile>().SetupTemperatureStats(-40);
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var orig = texture.Size() / 2f;
            var drawPos = projectile.Center - Main.screenPosition;
            var drawColor = new Color(30, 255, 30, 0);
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            if (PrimRenderer.renderProjTrails)
            {
                if (prim == null)
                {
                    prim = new PrimRenderer(mod.GetTexture("Effects/Prims/ThickerLine"), PrimRenderer.DefaultPass, (p) => new Vector2(projectile.width - p * projectile.width), (p) => drawColor * (1f - p),
                        drawOffset: projectile.Size / 2f);
                }
                prim.Draw(projectile.oldPos);
            }
            else
            {
                int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
                for (int i = 0; i < trailLength; i++)
                {
                    if (projectile.oldPos[i] == new Vector2(0f, 0f))
                        break;
                    float progress = 1f - 1f / trailLength * i;
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, null, drawColor * progress, projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            Main.spriteBatch.Draw(texture, drawPos, null, new Color(drawColor.R, drawColor.G, drawColor.B, 0), projectile.rotation, orig, projectile.scale, SpriteEffects.None, 0f);
            var spotlight = LegacyTextureCache.Lights[LightTex.Spotlight15x15];
            Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(drawColor.R, drawColor.G, drawColor.B, 0), projectile.rotation, spotlight.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}