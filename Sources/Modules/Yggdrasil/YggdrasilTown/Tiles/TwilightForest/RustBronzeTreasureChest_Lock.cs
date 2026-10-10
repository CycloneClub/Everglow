using Everglow.Yggdrasil.WorldGeneration;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public class RustBronzeTreasureChest_Lock : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSpelunker[Type] = true;
		Main.tileContainer[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileOreFinderPriority[Type] = 500;
		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.BasicChest[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.AvoidedByNPCs[Type] = true;
		TileID.Sets.InteractableByNPCs[Type] = true;
		TileID.Sets.IsAContainer[Type] = true;
		TileID.Sets.FriendlyFairyCanLureTo[Type] = true;

		DustType = DustID.Copper;
		AdjTiles = new int[] { TileID.Containers };

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
		TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
		TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
		TileObjectData.newTile.AnchorInvalidTiles = new int[]
		{
			TileID.MagicalIceBlock,
			TileID.Boulder,
			TileID.BouncyBoulder,
			TileID.LifeCrystalBoulder,
			TileID.RollingCactus,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(127, 96, 104));
	}

	public override ushort GetMapOption(int i, int j)
	{
		return (ushort)(TileUtils.SafeGetTile(i, j).TileFrameX / 36);
	}

	public override LocalizedText DefaultContainerName(int frameX, int frameY)
	{
		int option = frameX / 36;
		return this.GetLocalization("MapEntry" + option);
	}

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
	{
		return true;
	}

	public static string MapChestName(string name, int i, int j)
	{
		int left = i;
		int top = j;
		Tile tile = TileUtils.SafeGetTile(i, j);
		if (tile.TileFrameX % 36 != 0)
		{
			left--;
		}

		if (tile.TileFrameY != 0)
		{
			top--;
		}

		int chest = Chest.FindChest(left, top);
		if (chest < 0)
		{
			return Language.GetTextValue("LegacyChestType.0");
		}

		if (Main.chest[chest].name == string.Empty)
		{
			return name;
		}

		return name + ": " + Main.chest[chest].name;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 1;
	}

	public override bool RightClick(int i, int j)
	{
		if (UnLockLeft(i, j).TileFrameX == 0)
		{
			return false;
		}
		if (UnLockRight(i, j).TileFrameX == 0)
		{
			return false;
		}
		if (UnLockUp(i, j).TileFrameX == 0)
		{
			return false;
		}
		return FurnitureUtils.ChestRightClick(i, j);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = TileUtils.SafeGetTile(i, j);
		if (tile.TileFrameX == 0 && tile.TileFrameY % 36 == 0)
		{
			var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			Texture2D texture = ModAsset.GreenRelicBrick_BonusKey.Value;
			if (UnLockLeft(i, j).TileFrameX == 0)
			{
				spriteBatch.Draw(texture, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(16, 12) + new Vector2(-24, 0), null, new Color(1f, 1f, 1f, 0.3f), 0, texture.Size() * 0.5f, 1, SpriteEffects.None, 0);
			}
			else
			{
				spriteBatch.Draw(texture, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(16, 12) + new Vector2(-24, 0), null, new Color(0.3f, 0.1f, 0.1f, 0.6f), 0, texture.Size() * 0.5f, 1, SpriteEffects.None, 0);
			}

			if (UnLockRight(i, j).TileFrameX == 0)
			{
				spriteBatch.Draw(texture, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(16, 12) + new Vector2(24, 0), null, new Color(1f, 1f, 1f, 0.3f), 0, texture.Size() * 0.5f, 1, SpriteEffects.None, 0);
			}
			else
			{
				spriteBatch.Draw(texture, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(16, 12) + new Vector2(24, 0), null, new Color(0.3f, 0.1f, 0.1f, 0.6f), 0, texture.Size() * 0.5f, 1, SpriteEffects.None, 0);
			}

			if (UnLockUp(i, j).TileFrameX == 0)
			{
				spriteBatch.Draw(texture, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(16, 12) + new Vector2(0, -20), null, new Color(1f, 1f, 1f, 0.3f), 0, texture.Size() * 0.5f, 1, SpriteEffects.None, 0);
			}
			else
			{
				spriteBatch.Draw(texture, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(16, 12) + new Vector2(0, -20), null, new Color(0.3f, 0.1f, 0.1f, 0.6f), 0, texture.Size() * 0.5f, 1, SpriteEffects.None, 0);
			}
		}

		base.PostDraw(i, j, spriteBatch);
	}

	public Tile UnLockLeft(int i, int j)
	{
		Tile tile = TileUtils.SafeGetTile(i, j);
		Point topLeftPoint = new Point(i - tile.TileFrameX / 18, j - tile.TileFrameY % 36 / 18);
		Tile targetTile = TileUtils.SafeGetTile(topLeftPoint + new Point(-20, -75));
		return targetTile;
	}

	public Tile UnLockRight(int i, int j)
	{
		Tile tile = TileUtils.SafeGetTile(i, j);
		Point topLeftPoint = new Point(i - tile.TileFrameX / 18, j - tile.TileFrameY % 36 / 18);
		Tile targetTile = TileUtils.SafeGetTile(topLeftPoint + new Point(20, -75));
		return targetTile;
	}

	public Tile UnLockUp(int i, int j)
	{
		Tile tile = TileUtils.SafeGetTile(i, j);
		Point topLeftPoint = new Point(i - tile.TileFrameX / 18, j - tile.TileFrameY % 36 / 18);
		Tile targetTile = TileUtils.SafeGetTile(topLeftPoint + new Point(0, -105));
		return targetTile;
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		// UnLockUp(i, j).TileFrameX = 0;
	}
}
