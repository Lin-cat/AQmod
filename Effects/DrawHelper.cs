﻿using AQMod.Common.Graphics;
using AQMod.Content.Entities;
using AQMod.Effects.GoreNest;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Effects
{
    public sealed class DrawHelper : ModWorld
    {
        public static int Main_bgTop;

        public static class Hooks
        {
            public static float? PlayerDrawScale = null;

            internal static int LastScreenWidth;
            internal static int LastScreenHeight;

            internal static void Apply()
            {
                On.Terraria.Main.DrawProjectiles += DrawProjectiles;
                On.Terraria.Main.DrawPlayers += DrawPlayers;
                On.Terraria.Main.DrawPlayer_DrawAllLayers += DrawPlayer_DrawAllLayers;
                On.Terraria.Main.DrawNPCs += DrawNPCs;
                On.Terraria.Main.DrawTiles += DrawTiles;
                On.Terraria.Main.UpdateDisplaySettings += UpdateDisplaySettings;
            }

            private static void DrawPlayer_DrawAllLayers(On.Terraria.Main.orig_DrawPlayer_DrawAllLayers orig, Main self, Player drawPlayer, int projectileDrawPosition, int cHead)
            {
                if (PlayerDrawScale != null)
                {
                    var to = new Vector2((int)drawPlayer.position.X + drawPlayer.width / 2f, (int)drawPlayer.position.Y + drawPlayer.height);
                    to -= Main.screenPosition;
                    for (int i = 0; i < Main.playerDrawData.Count; i++)
                    {
                        DrawData data = Main.playerDrawData[i];
                        data.position -= (data.position - to) * (1f - PlayerDrawScale.Value);
                        data.scale *= PlayerDrawScale.Value;
                        Main.playerDrawData[i] = data;
                    }
                }
                orig(self, drawPlayer, projectileDrawPosition, cHead);
            }

            internal static void UpdateDisplaySettings(On.Terraria.Main.orig_UpdateDisplaySettings orig, Main self)
            {
                orig(self);
                if (!Main.gameMenu && !AQMod.Loading && Main.graphics.GraphicsDevice != null && !Main.graphics.GraphicsDevice.IsDisposed && Main.spriteBatch != null)
                {
                    AQGraphics.SetCullPadding();
                    LastScreenWidth = Main.screenWidth;
                    LastScreenHeight = Main.screenHeight;
                }
            }

            internal static void DrawNPCs(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
            {
                try
                {
                    NPCsBehindAllNPCs.drawingNow = true;
                    for (int i = 0; i < NPCsBehindAllNPCs.Count; i++)
                    {
                        Main.instance.DrawNPC(NPCsBehindAllNPCs.Indices[i], behindTiles);
                    }
                    NPCsBehindAllNPCs.Clear();
                }
                catch
                {
                    NPCsBehindAllNPCs?.Clear();
                    NPCsBehindAllNPCs = new DrawIndexCache();
                }
                if (!behindTiles)
                {
                    GoreNestRenderer.Render();
                    UltimateSwordRenderer.Render();
                }
                orig(self, behindTiles);
                if (behindTiles)
                {
                    try
                    {
                        ProjsBehindTiles.drawingNow = true;
                        if (ProjsBehindTiles != null)
                        {
                            for (int i = 0; i < ProjsBehindTiles.Count; i++)
                            {
                                Main.instance.DrawProj(ProjsBehindTiles.Indices[i]);
                            }
                        }
                        ProjsBehindTiles.Clear();
                    }
                    catch
                    {
                        ProjsBehindTiles?.Clear();
                        ProjsBehindTiles = new DrawIndexCache();
                    }
                }
            }

            internal static void DrawTiles(On.Terraria.Main.orig_DrawTiles orig, Main self, bool solidOnly, int waterStyleOverride)
            {
                if (!solidOnly)
                {
                    GoreNestRenderer.RefreshCoordinates();
                }
                orig(self, solidOnly, waterStyleOverride);
            }

            internal static void DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
            {
                BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
                AQMod.Particles.PreDrawProjectiles.Render();
                AQMod.Trails.PreDrawProjectiles.Render();
                Main.spriteBatch.End();
                orig(self);
            }

            internal static void DrawPlayers(On.Terraria.Main.orig_DrawPlayers orig, Main self)
            {
                orig(self);
                AQGraphics.SetCullPadding();
                BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
                for (int i = 0; i < CrabPot.maxCrabPots; i++)
                {
                    CrabPot.crabPots[i].Render();
                }
                global::AQMod.AQMod.Particles.PostDrawPlayers.Render();
                Main.spriteBatch.End();
            }
        }
        public static class BGStars
        {
            public static Vector2 GetRenderPosition(Star star)
            {
                return GetRenderPosition(new Vector2(star.position.X + Main.starTexture[star.type].Width * 0.5f, star.position.Y + Main.starTexture[star.type].Height * 0.5f));
            }
            public static Vector2 GetRenderPosition(Vector2 position)
            {
                return new Vector2(position.X * (Main.screenWidth / 800f),
                    position.Y * (Main.screenHeight / 600f) + Main_bgTop);
            }
        }

        public static DrawIndexCache NPCsBehindAllNPCs { get; private set; }
        public static DrawIndexCache ProjsBehindTiles { get; private set; }

        public override void Initialize()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                UltimateSwordRenderer.Initalize();
            }
        }

        internal static void Load()
        {
            Hooks.LastScreenWidth = 0;
            Hooks.LastScreenHeight = 0;
            NPCsBehindAllNPCs = new DrawIndexCache();
            ProjsBehindTiles = new DrawIndexCache();
        }

        internal static void Unload()
        {
            NPCsBehindAllNPCs?.Clear();
            NPCsBehindAllNPCs = null;
            ProjsBehindTiles?.Clear();
            ProjsBehindTiles = null;
        }

        internal static void DoUpdate()
        {
            UltimateSwordRenderer.Update();
        }
    }
}