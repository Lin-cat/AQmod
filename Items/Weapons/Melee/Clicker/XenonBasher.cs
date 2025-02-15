﻿using AQMod.Common.ID;
using AQMod.Content.Players;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable.Nature;
using AQMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee.Clicker
{
    public class XenonBasher : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.damage = 20;
            item.useTime = 8;
            item.useAnimation = 8;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(silver: 60);
            item.rare = ItemRarityID.Green;
            item.melee = true;
            item.knockBack = 1.25f;
            item.noMelee = true;
            item.shoot = ModContent.ProjectileType<XenonBasherProj>();
        }

        public override void HoldItem(Player player)
        {
            if (Main.cursorOverride <= 0 && !player.mouseInterface && Vector2.Distance(Main.MouseWorld, player.Center) < 320f)
            {
                player.GetModPlayer<PlayerCursorDyes>().VisibleCursorDye = CursorDyeID.WhackAZombie2;
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            var normal = Vector2.Normalize(position - player.Center);
            speedX = normal.X * 0.1f;
            speedY = normal.Y * 0.1f;
            if (Vector2.Distance(position, player.Center) > 320f)
            {
                position = player.Center + normal * 320f;
            }
            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<VineSword>());
            r.AddIngredient(ModContent.ItemType<XenonMushroom>(), 2);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}