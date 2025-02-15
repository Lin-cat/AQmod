﻿using AQMod.Assets;
using AQMod.Common.Configuration;
using AQMod.Common.ID;
using AQMod.Dusts;
using AQMod.Effects.Prims;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class UmystickMoon : ModProjectile
    {
        protected PrimRenderer prim;
        protected Color _glowClr;

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.scale = 0.75f;
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] == 0)
            {
                projectile.ai[0] = 1f;
                projectile.frame = Main.rand.Next(Main.projFrames[projectile.type]);
                projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                _glowClr = new Color(128, 70, 70, 0);
                if (projectile.frame == 1)
                    _glowClr = new Color(90, 128, 50, 0);
                else if (projectile.frame == 2)
                    _glowClr = new Color(70, 70, 128, 0);
            }
            projectile.rotation += projectile.velocity.Length() * 0.0157f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            var origin = frame.Size() / 2f;
            var center = projectile.Center;
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            if (PrimRenderer.renderProjTrails)
            {
                if (prim == null)
                {
                    prim = new PrimRenderer(mod.GetTexture("Effects/Prims/ThickTrail"), PrimRenderer.DefaultPass,
                        (p) => new Vector2(14f - p * 14f) * projectile.scale, (p) => _glowClr * (1f - p),
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
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(100, 100, 100, 0) * progress, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            if (AQConfigClient.Instance.EffectQuality >= 1f)
            {
                var glow = LegacyTextureCache.Lights[LightTex.Spotlight66x66];
                Main.spriteBatch.Draw(glow, center - Main.screenPosition, null, _glowClr, projectile.rotation, glow.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, frame, new Color(250, 250, 250, 160), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var center = projectile.Center;
            float size = projectile.width / 2f;
            if (Main.netMode != NetmodeID.Server)
            {
                AQSound.LegacyPlay(SoundType.Item, AQSound.Paths.MysticUmbrellaDestroy, projectile.Center, 0.5f, -0.25f);
            }
            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>());
                var n = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi).ToRotationVector2();
                Main.dust[d].position = center + n * Main.rand.NextFloat(0f, size);
                Main.dust[d].velocity = n * Main.rand.NextFloat(2f, 7f);
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 1.75f);
                Main.dust[d].color = _glowClr * Main.rand.NextFloat(0.8f, 2f);
            }
        }
    }
}