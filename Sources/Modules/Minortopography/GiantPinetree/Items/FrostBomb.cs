using Everglow.Minortopography.GiantPinetree.Projectiles;
using Terraria.DataStructures;

namespace Everglow.Minortopography.GiantPinetree.Items;
// TODO:翻译
// 释放缓慢飞行的冰球
// 右键丢下霜雷
// 使用左键会引爆鼠标附近的霜雷
public class FrostBomb : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 56;
		Item.damage = 10;
		Item.rare = ItemRarityID.Green;
		Item.useTime = 24;
		Item.useAnimation = 24;
		Item.autoReuse = true;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.shoot = ModContent.ProjectileType<FrostBall>();
		Item.shootSpeed = 7f;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 14;
	}
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.altFunctionUse == 2)
		{
			if (player.statMana < Item.mana * 2)
			{
				return false;
			}
			Projectile p0 = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<Projectiles.FrostBomb>(), damage * 2, knockback, player.whoAmI, 1);
			p0.rotation = Main.rand.NextFloat(6.283f);
			player.statMana -= Item.mana;
			return false;
		}
		return base.Shoot(player, source, position, velocity, type, damage, knockback);
	}
	public override bool AltFunctionUse(Player player)
	{
		return true;
	}
}
