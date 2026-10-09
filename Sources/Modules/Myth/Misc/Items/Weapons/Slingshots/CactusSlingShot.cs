using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Items.Weapons.Slingshots;

public class CactusSlingShot : SlingshotItem
{
	public override void SetDef()
	{
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Ranged.Slingshots.CactusSlingShot>();
		Item.damage = 9;
		Item.useTime = 23;
		Item.useAnimation = 23;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Cobweb, 14)
			.AddIngredient(ItemID.Cactus, 7)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
