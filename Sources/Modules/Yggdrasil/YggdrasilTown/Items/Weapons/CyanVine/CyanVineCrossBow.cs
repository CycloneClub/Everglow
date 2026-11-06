using Everglow.Commons.Templates.Weapons.CrossBow;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.CyanVine;

public class CyanVineCrossBow : CrossBowItem
{
	public override void SetDef()
	{
		Item.width = 74;
		Item.height = 34;
		Item.rare = ItemRarityID.White;
		Item.value = 3800;
		Item.useTime = 24;
		Item.useAnimation = 24;
		Item.damage = 20;
		Item.knockBack = 4f;
		CrossBowProjType = ModContent.ProjectileType<CyanVineCrossBow_Proj>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<CyanVineBar>(), 26)
			.AddIngredient(ModContent.ItemType<StoneDragonScaleWood>(), 12)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
