using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Items.Weapons.Slingshots;

public class RubySlingshot : SlingshotItem
{
	public override void SetDef()
	{
		projType = ModContent.ProjectileType<Projectiles.Weapon.Ranged.Slingshots.RubySlingshot>();
		Item.damage = 23;
		Item.width = 38;
		Item.height = 36;
		Item.useTime = 35;
		Item.useAnimation = 35;
		Item.rare = ItemRarityID.Orange;
		Item.value = Item.sellPrice(0, 0, 17, 0);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Ruby, 8)
			.AddIngredient(ItemID.GoldBar, 6)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
