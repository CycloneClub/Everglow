using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Items.Weapons.Slingshots;

public class StarSlingshot : SlingshotItem
{
	public override void SetDef()
	{
		Item.damage = 18;
		Item.crit = 12;
		Item.width = 32;
		Item.height = 30;
		projType = ModContent.ProjectileType<Projectiles.Weapon.Ranged.Slingshots.StarSlingshot>();
		Item.useTime = 24;
		Item.useAnimation = 24;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(0, 1, 50, 0);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.FallenStar, 25)
			.AddIngredient(ItemID.SunplateBlock, 30)
			.AddTile(TileID.SkyMill)
			.Register();
	}
}
