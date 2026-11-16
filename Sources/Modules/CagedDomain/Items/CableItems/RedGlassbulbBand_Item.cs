using Everglow.Commons.TileHelper;

namespace Everglow.CagedDomain.Items.CableItems;

/// <summary>
/// 经过电工简易改装的圣诞树彩灯，现在可以摆放到其他地方了
/// </summary>
public class RedGlassbulbBand_Item : CableTileItem
{
	public override int TileType => ModContent.TileType<Tiles.CableTiles.RedGlassbulbBand_Bulb>();

	public override void SetDefaults()
	{
		base.SetDefaults();
		Item.width = 24;
		Item.height = 28;
		Item.value = 40;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.RedLight, 15)
			.AddIngredient(ItemID.Wire, 5)
			.AddTile(TileID.WorkBenches)
			.Register();
		CreateRecipe()
			.AddIngredient(ItemID.RedLights)
			.AddIngredient(ItemID.Wire, 2)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
