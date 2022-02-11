﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions
{
    public class Baguette : ModItem, IDedicatedItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.UseSound = SoundID.Item2;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = AQItem.Rarities.DedicatedItem;
            item.value = Item.buyPrice(silver: 80);
            item.buffType = BuffID.WellFed;
            item.buffTime = 216000;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].mod == "Terraria" && tooltips[i].Name == "ItemName")
                {
                    tooltips[i].overrideColor = Color.Lerp(new Color(255, 222, 150), new Color(170, 130, 80), AQUtils.Wave(Main.GlobalTime * 10f, 0f, 1f));
                    return;
                }
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                TooltipText.DrawNarrizuulText(line);
                return false;
            }
            return true;
        }

        public override bool CanBurnInLava()
        {
            return true;
        }

        Color IDedicatedItem.Color => new Color(187, 142, 42, 255);
    }
}