using Everglow.Myth.LanternMoon.Buffs;
using Everglow.Myth.LanternMoon.Projectiles.Weapons;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Items.Weapons;

/// <summary>
/// Mark target with a lantern label.
/// Do at least 500 damage to a labeled target will remove the label and trigger an explosion.
/// </summary>
public class GoldenLotusStaff : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 42;
		Item.height = 42;
		Item.useAnimation = 26;
		Item.useTime = 26;
		Item.knockBack = 1.5f;
		Item.damage = 50;
		Item.rare = ItemRarityID.Lime;
		Item.autoReuse = true;
		Item.UseSound = SoundID.Item1;
		Item.DamageType = DamageClass.Summon;
		Item.mana = 24;
		Item.noMelee = true;
		Item.shootSpeed = 14f;
		Item.shoot = ModContent.ProjectileType<GoldenLotusStaff_Proj>();
		Item.value = 15000;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		player.AddBuff(ModContent.BuffType<GoldenLotusStaff_Buff>(), 6);
		return base.Shoot(player, source, position, velocity, type, damage, knockback);
	}
}
