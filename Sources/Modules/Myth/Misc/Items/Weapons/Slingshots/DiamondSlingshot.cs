using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Items.Weapons.Slingshots;

public class DiamondSlingshot : SlingshotItem
{
	public override void SetDef()
	{
		projType = ModContent.ProjectileType<Projectiles.Weapon.Ranged.Slingshots.DiamondSlingshot>();
		Item.damage = 25;
		Item.width = 38;
		Item.height = 36;
		Item.useTime = 33;
		Item.useAnimation = 33;
		Item.rare = ItemRarityID.Orange;
		Item.value = Item.sellPrice(0, 0, 10, 0);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Diamond, 8)
			.AddIngredient(ItemID.PlatinumBar, 6)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
