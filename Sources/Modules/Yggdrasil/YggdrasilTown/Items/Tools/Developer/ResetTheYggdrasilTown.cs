using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;

public class ResetTheYggdrasilTown : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 30;
		Item.useTurn = true;
		Item.useAnimation = 4;
		Item.useTime = 4;
		Item.autoReuse = false;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	public override bool CanUseItem(Player player)
	{
		TileUtils.PlaceRectangleAreaOfBlock(20, Main.maxTilesY - 1500, Main.maxTilesX - 20, Main.maxTilesY, -1);
		YggdrasilTownGeneration.BuildYggdrasilTown();
		return false;
	}
}
