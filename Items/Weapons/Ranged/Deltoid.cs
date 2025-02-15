﻿using AQMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class Deltoid : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.Glowmask(() => new Color(200, 200, 200, 0) * AQUtils.Wave(Main.GlobalTime * 6f, 0.9f, 1f));
        }

        public override void SetDefaults()
        {
            item.damage = 23;
            item.ranged = true;
            item.useTime = 38;
            item.useAnimation = 38;
            item.width = 20;
            item.height = 30;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = AQItem.RarityDemonSiege;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 2.8f;
            item.useAmmo = AmmoID.Arrow;
            item.UseSound = SoundID.Item5;
            item.value = AQItem.Prices.DemonSiegeWeaponValue;
            item.noMelee = true;
            item.autoReuse = true;
            item.knockBack = 3f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return AQItem.DemonSiegeItemAlpha(lightColor);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), DeltoidArrow.ArrowProjectileIDToHamaYumiProjectileID(type), damage, knockBack, player.whoAmI, 0f, type);
            return false;
        }
    }
}