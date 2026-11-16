using Everglow.Commons.TileHelper;

namespace Everglow.CagedDomain.Items.CableItems;

/// <summary>
/// 锁链绳
/// </summary>
public class ChainCable_Item : CableTileItem
{
	public override int TileType => ModContent.TileType<Tiles.CableTiles.ChainCable>();

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
			.AddIngredient(ItemID.Chain, 15)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
