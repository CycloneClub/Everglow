using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Items.Weapons.Slingshots;

public class JungleSlingshot : SlingshotItem
{
	// TODO:丛林孢子弹弓:划过的轨迹以及爆炸散开的范围都会随机施加中毒
	public override void SetDef()
	{
		Item.damage = 22;
		Item.crit = 4;
		Item.width = 34;
		Item.height = 34;
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Ranged.Slingshots.JungleSlingshot>();

		Item.rare = ItemRarityID.Green;
		Item.value = Item.sellPrice(0, 0, 80, 0);
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Vine, 8)
			.AddIngredient(ItemID.Stinger, 6)
			.AddIngredient(ItemID.JungleSpores, 8)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
