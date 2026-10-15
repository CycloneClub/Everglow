using Everglow.CagedDomain.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class UnionGiantChandelier_Style1_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.width = 42;
		Item.height = 34;
		Item.maxStack = Item.CommonMaxStack;
		Item.createTile = ModContent.TileType<UnionGiantChandelier_Style1>();
		Item.placeStyle = 0;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.useAnimation = 15;
		Item.useTime = 15;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.consumable = true;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
	}

	public override void HoldItem(Player player)
	{
		Main.placementPreview = true;
	}

	public override bool CanUseItem(Player player)
	{
		var uChandelier = TileLoader.GetTile(ModContent.TileType<UnionGiantChandelier_Style1>()) as UnionGiantChandelier_Style1;
		if (uChandelier != null)
		{
			int x = (int)(Main.MouseWorld.X / 16 - 3);
			int y = (int)(Main.MouseWorld.Y / 16);
			if (uChandelier.CanPlaceAtTopLeft(x, y))
			{
				uChandelier.PlaceOriginAtTopLeft(x, y);
				Item.stack--;
				return false;
			}
		}
		return false;
	}

	public override bool? UseItem(Player player)
	{
		return false;
	}
}
