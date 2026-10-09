using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Items.Weapons.Slingshots;

public class WoodSlingshot : SlingshotItem
{
	public override void SetDef()
	{
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Ranged.Slingshots.WoodSlingshot>();
		Item.useTime = 26;
		Item.useAnimation = 26;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Cobweb, 14)
			.AddIngredient(ItemID.Wood, 7)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
