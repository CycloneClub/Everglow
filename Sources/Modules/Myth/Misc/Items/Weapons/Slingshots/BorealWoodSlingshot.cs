using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Items.Weapons.Slingshots;

public class BorealWoodSlingshot : SlingshotItem
{
	public override void SetDef()
	{
		projType = ModContent.ProjectileType<Projectiles.Weapon.Ranged.Slingshots.BorealWoodSlingshot>();
		Item.damage = 8;
		Item.useTime = 23;
		Item.useAnimation = 23;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Cobweb, 14)
			.AddIngredient(ItemID.BorealWood, 7)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
