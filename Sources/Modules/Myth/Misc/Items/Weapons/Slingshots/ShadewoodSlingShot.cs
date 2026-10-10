using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Items.Weapons.Slingshots;

public class ShadewoodSlingShot : SlingshotItem
{
	public override void SetDef()
	{
		projType = ModContent.ProjectileType<Projectiles.Weapon.Ranged.Slingshots.ShadewoodSlingShot>();
		Item.damage = 9;
		Item.useTime = 22;
		Item.useAnimation = 22;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Cobweb, 14)
			.AddIngredient(ItemID.Shadewood, 7)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
