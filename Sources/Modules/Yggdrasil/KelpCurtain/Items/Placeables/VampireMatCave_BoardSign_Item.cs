using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables;

public class VampireMatCave_BoardSign_Item : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<VampireMatCave_BoardSign>());
		Item.width = 18;
		Item.height = 34;
		Item.value = 10;
	}
	public override void HoldItem(Player player)
	{
		Main.placementPreview = true;
	}

	public override bool CanUseItem(Player player)
	{
		var vS = TileLoader.GetTile(ModContent.TileType<VampireMatCave_BoardSign>()) as VampireMatCave_BoardSign;
		if (vS != null)
		{
			int x = (int)(Main.MouseWorld.X / 16);
			int y = (int)(Main.MouseWorld.Y / 16);
			vS.PlaceAtTileObjectDataOrigin(x, y);
			Item.stack--;
			return false;
		}
		return false;
	}
}
