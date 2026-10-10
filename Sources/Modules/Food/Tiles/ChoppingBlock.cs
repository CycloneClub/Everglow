using Everglow.Food.Dusts;
using Everglow.Food.Items.Ingredients;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Food.Tiles;

public class ChoppingBlock : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		DustType = DustID.Pearlwood;

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Platform | AnchorType.Table | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			18,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

		// The following 3 lines are needed if you decide to add more styles and stack them vertically
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = true;

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1); // Facing right will use the second texture style
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(226, 196, 158));
	}

	public override bool RightClick(int i, int j)
	{
		Item item = Main.LocalPlayer.HeldItem;
		FoodIngredientItem foodIngredientItem = item.ModItem as FoodIngredientItem;
		var tile = Main.tile[i, j];
		if (foodIngredientItem != null)
		{
			foodIngredientItem.SliceDown((int)(i - (tile.TileFrameX % 54 - 18f) / 18f), j);
		}
		if (item.type == ItemID.SpicyPepper)
		{
			item.stack--;
			if (item.stack <= 0)
			{
				item.TurnToAir();
			}
			Item.NewItem(null, new Vector2((int)(i - (tile.TileFrameX % 54 - 18f) / 18f), j) * 16 + new Vector2(8, -8), ModContent.ItemType<SpicyPepperRing>(), 1);
			for (int t = 0; t < 12; t++)
			{
				Dust dust = Dust.NewDustDirect(new Vector2((int)(i - (tile.TileFrameX % 54 - 18f) / 18f), j) * 16, 16, 16, ModContent.DustType<SpicyPepperDust>());
				dust.velocity.Y -= 3;
			}
		}
		return base.RightClick(i, j);
	}

	public override void MouseOver(int i, int j)
	{
		Item item = Main.LocalPlayer.HeldItem;
		FoodIngredientItem foodIngredientItem = item.ModItem as FoodIngredientItem;
		if (foodIngredientItem != null)
		{
			if (foodIngredientItem.SlicedItemType > 0)
			{
				Main.instance.MouseText("[i:" + item.type + "] → [i:" + foodIngredientItem.SlicedItemType + "]", ItemRarityID.White);
			}
		}
		if (item.type == ItemID.SpicyPepper)
		{
			Main.instance.MouseText("[i:" + item.type + "] → [i:" + ModContent.ItemType<SpicyPepperRing>() + "]", ItemRarityID.White);
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return base.PreDraw(i, j, spriteBatch);
	}
}
