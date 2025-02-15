﻿using AQMod.Tiles.CrabCrevice;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials
{
    public class CrustaciumBlob : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Blue;
            item.createTile = ModContent.TileType<CrustaciumFlesh>();
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }

        public override void CaughtFishStack(ref int stack)
        {
            stack = Main.rand.Next(16) + 1;
        }
    }
}