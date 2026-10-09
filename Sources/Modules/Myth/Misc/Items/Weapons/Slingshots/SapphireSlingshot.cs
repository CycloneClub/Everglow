using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Items.Weapons.Slingshots;

public class SapphireSlingshot : SlingshotItem
{
	public override void SetDef()
	{
		projType = ModContent.ProjectileType<Projectiles.Weapon.Ranged.Slingshots.SapphireSlingshot>();
		Item.damage = 19;
		Item.width = 38;
		Item.height = 36;
		Item.useTime = 38;
		Item.useAnimation = 38;
		Item.rare = ItemRarityID.Orange;
		Item.value = Item.sellPrice(0, 0, 12, 0);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Sapphire, 8)
			.AddIngredient(ItemID.SilverBar, 6)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
