using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Items.Weapons.Slingshots;

public class EbonwoodSlingshot : SlingshotItem
{
	public override void SetDef()
	{
		ProjType = ModContent.ProjectileType<Projectiles.Weapon.Ranged.Slingshots.EbonwoodSlingshot>();
		Item.damage = 9;
		Item.useTime = 22;
		Item.useAnimation = 22;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Cobweb, 14)
			.AddIngredient(ItemID.Ebonwood, 7)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
