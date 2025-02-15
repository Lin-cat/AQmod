﻿using AQMod.Common;
using AQMod.Common.Graphics.PlayerEquips;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.Arachnotron
{
    [AutoloadEquip(EquipType.Head)]
    public class AArachnotronVisor : ModItem
    {
        public override bool Autoload(ref string name)
        {
            name = "ArachnotronVisor";
            return base.Autoload(ref name);
        }

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                if (GlowmaskData.ItemToGlowmask == null)
                {
                    GlowmaskData.ItemToGlowmask = new Dictionary<int, GlowmaskData>();
                }
                var glow = new AQUtils.ItemGlowmask(() => new Color(250, 250, 250, 0));
                GlowmaskData.ItemToGlowmask.Add(item.type, new GlowmaskData(mod.GetTexture("Items/Armor/Arachnotron/ArachnotronVisor_Glow"), glow));
                AQMod.ArmorOverlays.AddHeadOverlay<AArachnotronVisor>(new ArachnotronVisorOverlay());
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 10;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.sellPrice(gold: 7, silver: 50);
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ABArachnotronRibcage>() && legs.type == ModContent.ItemType<ArachnotronRevvers>();
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeCrit += 10;
            player.meleeDamage += 0.1f;
            player.minionDamage += 0.1f;
            player.nightVision = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = AQMod.GetText("ArmorSetBonus.Arachnotron", Keybinds.GetText(AQMod.Triggers.ArmorSetBonus));
            player.GetModPlayer<AQPlayer>().setArachnotron = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.SpiderMask);
            r.AddIngredient(ItemID.HallowedBar, 12);
            r.AddIngredient(ItemID.SoulofFright, 4);
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}