using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class RestrictionDeviceRE01 : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonWeapons;

	public override string Texture => $"Terraria/Images/Item_{ItemID.XenoStaff}";

	public override void SetStaticDefaults()
	{
		Item.staff[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;

		Item.DamageType = DamageClass.Summon;
		Item.damage = 18;
		Item.knockBack = 2f;
		Item.mana = 15;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = Item.useAnimation = 21;
		Item.noMelee = true;
		Item.shoot = ModContent.ProjectileType<RestrictionDroneRE01>();
		Item.buffType = ModContent.BuffType<RestrictionDroneRE01Buff>();
		Item.shootSpeed = 1f;

		Item.rare = ItemRarityID.Pink;
		Item.value = Item.buyPrice(gold: 4);
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.whoAmI == Main.myPlayer)
		{
			player.AddBuff(Item.buffType, 2);
			var drone = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
			drone.originalDamage = Item.damage;
		}
		return false;
	}
}
