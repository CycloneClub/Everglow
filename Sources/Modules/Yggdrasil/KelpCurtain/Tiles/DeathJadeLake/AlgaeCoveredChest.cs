using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class AlgaeCoveredChest : ModTile, ITileFluentlyDrawn
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

		DustType = ModContent.DustType<AlgaeCoveredChestDust>();
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
		return (ushort)(Main.tile[i, j].TileFrameX / 36);
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
		Tile tile = Main.tile[i, j];
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
		return FurnitureUtils.ChestRightClick(i, j);
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		Tile tile = Main.tile[i, j];
		int left = i;
		int top = j;
		if (tile.TileFrameX % 36 != 0)
		{
			left--;
		}

		if (tile.TileFrameY != 0)
		{
			top--;
		}

		int chest = Chest.FindChest(left, top);
		player.cursorItemIconID = -1;
		if (chest < 0)
		{
			player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
		}
		else
		{
			string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY); // This gets the ContainerName text for the currently selected language
			player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
			if (player.cursorItemIconText == defaultName)
			{
				player.cursorItemIconID = ModContent.ItemType<AlgaeCoveredChest_Item>();
				if (Main.tile[left, top].TileFrameX / 36 == 1)
				{
					// player.cursorItemIconID = ModContent.ItemType<>();
				}

				player.cursorItemIconText = string.Empty;
			}
		}

		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
	}

	public override void MouseOverFar(int i, int j)
	{
		MouseOver(i, j);
		Player player = Main.LocalPlayer;
		if (player.cursorItemIconText == string.Empty)
		{
			player.cursorItemIconEnabled = false;
			player.cursorItemIconID = ItemID.None;
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.tile[i, j + 1].TileType != Type)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		if (Main.tile[i, j - 1].TileType != Type)
		{
			// Alga at back
			int xLength = GetFixedRandomNumber(i + j, 2) + 1; // 1~2
			Texture2D tex = TextureAssets.Tile[Type].Value;
			var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			Vector2 drawPos = new Point(i, j + 2).ToWorldCoordinates(zero + new Vector2(8)) - Main.screenPosition;
			for (int x = 0; x < xLength; x++)
			{
				for (int y = 0; y < 7; y++)
				{
					int frameXChoose = GetFixedRandomNumber(i + x + j, 4) + 6;
					Rectangle frame = new Rectangle(36 + frameXChoose * 8, 106 - y * 6, 6, 6);
					float modifyX = MathF.Sin(i + x + y + (float)Main.time / 24f) * 1.5f;
					spriteBatch.Draw(tex, drawPos + new Vector2(modifyX, -y * 6), frame, Lighting.GetColor(i, j), 0, new Vector2(4, 6), 1f, SpriteEffects.None, 0);
				}
			}
		}
		return base.PreDraw(i, j, spriteBatch);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		base.PostDraw(i, j, spriteBatch);
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		DrawAlgae(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// Draw a piece of lotus
	/// </summary>
	private void DrawAlgae(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		int height = 7;

		var tile = Main.tile[tilePos];
		if (!(tile.TileType == Type && tile.HasTile))
		{
			return;
		}
		ushort type = tile.TileType;

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}
		int paint = Main.tile[tilePos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.AlgaeCoveredChest_Path, type, 1, paint, tileDrawing);
		tex ??= ModAsset.AlgaeCoveredChest.Value;
		for (int i = 0; i < 4; i++)
		{
			var lastOffset = new Vector2(0, 8);
			for (int j = 0; j < 6; j++)
			{
				int frameX = GetFixedRandomNumber((tilePos.X + GetFixedRandomNumber(i, 6)) % 512, 6) + 10;
				var frame = new Rectangle(36 + frameX * 8, 106 - j * 2, 8, 2);
				int totalPushTime = 140;
				float pushForcePerFrame = 0.96f;
				float windCycle = 0;
				if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
				{
					windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);
				}
				float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
				windCycle += highestWindGridPushComplex;
				float rotation = windCycle * 0.18f;
				rotation -= lastOffset.X / (42f + j * 3f);
				rotation += MathF.Sin((i * 6 + j) / (float)height * MathHelper.Pi) * MathF.Sin(tilePos.X * 2 + tilePos.Y - j * 5f + (float)Main.time / 20f) * 0.12f;
				rotation += MathF.Sin((i * 6 + j) / (float)height * MathHelper.Pi) * MathF.Sin(tilePos.X * 2 + tilePos.Y + (float)Main.time / 20f) * 2f / (height + 25);
				if (j <= 3)
				{
					rotation *= j / 3f;
				}
				float rotationToOffsetX = rotation * 8;
				var tileLight = Lighting.GetColor(tilePos + new Point(0, -(int)(j / 5f)));

				// 支持发光涂料
				tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y - j, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
				tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y - j, tile, type, 0, 0, tileLight);
				if (height > 10 && height - j <= 10)
				{
					tileLight *= (height - j) / 20f + 0.5f;
				}
				var origin = new Vector2(2, 2);
				var drawPos = drawCenterPos + lastOffset + new Vector2(i * 4 - 8 + rotationToOffsetX, 0);
				var tileSpriteEffect = SpriteEffects.None;
				spriteBatch.Draw(tex, drawPos, frame, tileLight, 0, origin, 1f, tileSpriteEffect, 0f);
				lastOffset += new Vector2(0, -2).RotatedBy(rotation);
			}
		}

		int xLength = GetFixedRandomNumber(tilePos.X + tilePos.Y, 2) + 2; // 2~3
		for (int i = 0; i < xLength; i++)
		{
			var lastOffset = new Vector2(0, 8);
			for (int j = 0; j < 11; j++)
			{
				int frameX = GetFixedRandomNumber((tilePos.X + GetFixedRandomNumber(i, 6)) % 512, 6);
				var frame = new Rectangle(36 + frameX * 8, 106 - j * 2, 8, 2);
				int totalPushTime = 140;
				float pushForcePerFrame = 0.96f;
				float windCycle = 0;
				if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
				{
					windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);
				}
				float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
				windCycle += highestWindGridPushComplex;
				float rotation = windCycle * 0.18f;
				rotation -= lastOffset.X / (42f + j * 3f);
				rotation += MathF.Sin((i * 6 + j) / (float)height * MathHelper.Pi) * MathF.Sin(tilePos.X * 2 + tilePos.Y - j * 5f + (float)Main.time / 20f) * 0.12f;
				rotation += MathF.Sin((i * 6 + j) / (float)height * MathHelper.Pi) * MathF.Sin(tilePos.X * 2 + tilePos.Y + (float)Main.time / 20f) * 2f / (height + 25);
				if (j <= 3)
				{
					rotation *= j / 3f;
				}
				float rotationToOffsetX = rotation * 8;
				var tileLight = Lighting.GetColor(tilePos + new Point(0, -(int)(j / 5f)));

				// 支持发光涂料
				tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y - j, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
				tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y - j, tile, type, 0, 0, tileLight);
				if (height > 10 && height - j <= 10)
				{
					tileLight *= (height - j) / 20f + 0.5f;
				}
				var origin = new Vector2(2, 2);
				var drawPos = drawCenterPos + lastOffset + new Vector2((i - (xLength / 2f - 0.5f)) * (24 / xLength) + rotationToOffsetX, 0);
				var tileSpriteEffect = SpriteEffects.None;
				spriteBatch.Draw(tex, drawPos, frame, tileLight, 0, origin, 1f, tileSpriteEffect, 0f);
				lastOffset += new Vector2(0, -2).RotatedBy(rotation);
			}
		}
	}

	public int GetFixedRandomNumber(int seed, int max)
	{
		Random random = new Random(seed);
		return random.Next(0, max);
	}
}
