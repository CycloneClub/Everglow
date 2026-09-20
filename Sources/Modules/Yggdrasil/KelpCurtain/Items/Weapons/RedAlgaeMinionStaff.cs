using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class RedAlgaeMinionStaff : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonWeapons;

	public override void SetStaticDefaults()
	{
		Item.staff[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.DamageType = DamageClass.Summon;
		Item.damage = 42;
		Item.knockBack = 2;
		Item.mana = 27;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = Item.useAnimation = 27;
		Item.noMelee = true;

		Item.rare = ItemRarityID.Orange;
		Item.value = 35000;

		Item.shoot = ModContent.ProjectileType<CrimsonMoonAlgaeSummonStaff_minion>();
		Item.shootSpeed = 12f;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		player.AddBuff(ModContent.BuffType<CrimsonMoonAlgaeSummonStaff_Buff>(), 360000000);
		Projectile proj = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.zeroVector, type, damage, knockback, player.whoAmI);
		proj.spriteDirection = -1;
		if (Main.MouseWorld.X < player.Center.X)
		{
			proj.spriteDirection = 1;
		}
		return false;
	}

	public override bool CanUseItem(Player player)
	{
		if (player.maxMinions - player.GetSlotsMinions() < 1)
		{
			return false;
		}
		return base.CanUseItem(player);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
		.AddIngredient(ModContent.ItemType<JadeLakeRedAlgae_Item>(), 15)
		.AddIngredient(ModContent.ItemType<CrimsonMoonSap>(), 1)
		.AddTile(TileID.WorkBenches)
		.Register();
	}
}
