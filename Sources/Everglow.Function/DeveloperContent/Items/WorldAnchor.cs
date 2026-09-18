using Everglow.Commons.Utilities;
using Terraria;

namespace Everglow.Commons.DeveloperContent.Items;

/// <summary>
/// Visulalize the data of mouse-covered-tile.
/// </summary>
public class WorldAnchor : ModItem
{
	public bool EnableResidentEffect = false;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
	}

	public override void HoldItem(Player player)
	{
		int i = Main.MouseWorld.ToTileCoordinates().X;
		int j = Main.MouseWorld.ToTileCoordinates().Y;
		if (Main.mapFullscreen)
		{
			Vector2 mouseFromCenter = Main.MouseScreen - new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
			Vector2 mouseMap = Main.mapFullscreenPos + mouseFromCenter / Main.mapFullscreenScale * Main.GameViewMatrix.Zoom;
			i = (int)mouseMap.X;
			j = (int)mouseMap.Y;
		}

		string text = string.Empty;
		var tile = TileUtils.SafeGetTile(i, j);
		if (tile.HasTile)
		{
			text += "TileType :" + tile.TileType;
			text += " " + TileID.Search.GetName(tile.TileType);
		}
		text += "\n[" + i + ", " + j + "]\n";
		text += "[" + i / (float)Main.maxTilesX + ", " + j / (float)Main.maxTilesY + "]";
		Main.instance.MouseText(text);
	}
}
