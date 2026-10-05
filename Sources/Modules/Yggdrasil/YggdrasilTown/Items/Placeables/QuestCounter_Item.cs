using Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class QuestCounter_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.width = 58;
		Item.height = 34;
		Item.maxStack = Item.CommonMaxStack;
		Item.createTile = ModContent.TileType<QuestCounter>();
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
		var mCounter = TileLoader.GetTile(ModContent.TileType<QuestCounter>()) as QuestCounter;
		if (mCounter != null)
		{
			int x = (int)(Main.MouseWorld.X / 16 - 11);
			int y = (int)(Main.MouseWorld.Y / 16 - 11);
			mCounter.PlaceOriginAtTopLeft(x, y);
			Item.stack--;
			return false;
		}
		return false;
	}

	public override bool? UseItem(Player player)
	{
		return false;
	}
}
