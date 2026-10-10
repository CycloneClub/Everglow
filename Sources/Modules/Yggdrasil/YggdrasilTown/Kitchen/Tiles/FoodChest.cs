using Everglow.Food.Items.Ingredients;
using Everglow.Yggdrasil.YggdrasilTown.Kitchen.KitchenSystem;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ObjectData;
using Terraria.UI;

namespace Everglow.Yggdrasil.YggdrasilTown.Kitchen.Tiles;

public class FoodChest : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileNoAttach[Type] = false;
		TileID.Sets.DisableSmartCursor[Type] = true;

		DustType = DustID.DynastyWood;
		AdjTiles = new int[] { TileID.Benches };
		MinPick = 99999999;

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(153, 110, 110));
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		noBreak = true;
		return false;
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override bool RightClick(int i, int j)
	{
		var tile = Main.tile[i, j];
		if (!KitchenSystemUI.Started)
		{
			return false;
		}
		Point frameCoord = new Point((tile.TileFrameX - tile.TileFrameX % 36) / 36, (tile.TileFrameY - tile.TileFrameY % 36) / 36);
		int style = frameCoord.X + frameCoord.Y * 16;
		int itemType = GetStyleItemType(style);
		if (itemType != -1)
		{
			int itemCountInventory = 0;
			foreach (var item in Main.LocalPlayer.inventory)
			{
				if (!item.IsAir && item.type == itemType && item.stack > 0)
				{
					itemCountInventory += item.stack;
				}
			}
			if (itemCountInventory < 5)
			{
				Item.NewItem(null, new Point(i, j).ToWorldCoordinates(), itemType, 1);
				KitchenSystemUI.AddScore(-10);
			}
			else
			{
				CombatText.NewText(new Rectangle(i * 16, j * 16, 16, 16), Color.LightCoral, "You can't hold one ingerdient more than 5.");
			}
		}

		return false;
	}

	public static int GetStyleItemType(int style)
	{
		switch (style)
		{
			case 0:
				return ModContent.ItemType<Eggplant>();
			case 1:
				return ModContent.ItemType<Doubanjiang>();
			case 2:
				return ModContent.ItemType<FrogMeat>();
			case 3:
				return ModContent.ItemType<Garlic>();
			case 4:
				return ModContent.ItemType<Poultry>();
			case 5:
				return ModContent.ItemType<RawEgg>();
			case 6:
				return ModContent.ItemType<RawSteak>();
			case 7:
				return ModContent.ItemType<Rice>();
			case 8:
				return ModContent.ItemType<Scallion>();
			case 9:
				return ModContent.ItemType<SichuanPepper>();
			case 10:
				return ModContent.ItemType<Tofu>();
			case 11:
				return ItemID.SpicyPepper;
			case 12:
				return ModContent.ItemType<Gelatin_Sheet>();
		}
		return -1;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX % 36 != 18 || tile.TileFrameY % 36 != 18)
		{
			return;
		}
		Vector2 offset = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			offset = Vector2.zeroVector;
		}
		Vector2 drawPos = new Point(i, j).ToWorldCoordinates(0, 0) - Main.screenPosition + offset;

		Point frameCoord = new Point((tile.TileFrameX - tile.TileFrameX % 36) / 36, (tile.TileFrameY - tile.TileFrameY % 36) / 36);
		int style = frameCoord.X + frameCoord.Y * 16;
		int itemType = GetStyleItemType(style);

		if (itemType >= 0)
		{
			Item item = new Item(itemType);
			Color color = Lighting.GetColor(i, j) * 1.5f;
			Texture2D food = TextureAssets.Item[itemType].Value;
			Rectangle frame = (Main.itemAnimations[itemType] == null) ? food.Frame() : Main.itemAnimations[itemType].GetFrame(food);
			float scale;
			ItemSlot.DrawItem_GetColorAndScale(item, 0.5f, ref color, 20000, ref frame, out color, out scale);
			spriteBatch.Draw(food, drawPos, frame, color, 0f, frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);
		}
		if (!KitchenSystemUI.Started)
		{
			itemType = ItemID.BowTopper;
			Item item = new Item(itemType);
			Color color = Lighting.GetColor(i, j) * 0.5f;
			Texture2D food = TextureAssets.Item[itemType].Value;
			Rectangle frame = (Main.itemAnimations[itemType] == null) ? food.Frame() : Main.itemAnimations[itemType].GetFrame(food);
			float scale;
			ItemSlot.DrawItem_GetColorAndScale(item, 1f, ref color, 20000, ref frame, out color, out scale);
			spriteBatch.Draw(food, drawPos, frame, color, 0f, frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);
		}
	}
}
