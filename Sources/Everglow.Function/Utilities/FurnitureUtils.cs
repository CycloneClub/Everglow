using Everglow.Commons.TileHelper;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Drawing;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Commons.Utilities;

public static class FurnitureUtils
{
	public static void BannerFluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		int top = pos.Y - Main.tile[pos].TileFrameY / 18;
		HangingObjectFluentDraw(screenPosition, pos, spriteBatch, tileDrawing, new Point(pos.X, top), -4);
	}

	public static void LanternFluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		int top = pos.Y - Main.tile[pos].TileFrameY / 18;
		HangingObjectFluentDraw(screenPosition, pos, spriteBatch, tileDrawing, new Point(pos.X, top), 0);
	}

	public static void Chandelier3x3FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		int left = Main.tile[pos].TileFrameX / 18;
		left %= 3;
		left = pos.X - left;
		int top = pos.Y - Main.tile[pos].TileFrameY / 18;
		HangingObjectFluentDraw(screenPosition, pos, spriteBatch, tileDrawing, new Point(left, top), 0, 0.11f);
	}

	/// <summary>
	/// 用于悬挂类物块的FluentDraw，用于带摆动绘制，适配油漆
	/// </summary>
	/// <param name="screenPosition">屏幕坐标</param>
	/// <param name="pos">该格物块的物块坐标</param>
	/// <param name="spriteBatch">批量雪碧</param>
	/// <param name="tileDrawing">TileDrawing工具类实例</param>
	/// <param name="topLeft">物块整体左上角的坐标</param>
	/// <param name="swayOffset">用于旗帜类物块，摇曳时底部物块会有类似“卷起来”的效果，对于吊灯应直接设置为0</param>
	public static void HangingObjectFluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing, Point topLeft, float swayOffset = -4f, float swayStrength = 0.15f)
	{
		var tile = Main.tile[pos];
		var tileData = TileObjectData.GetTileData(tile.type, 0);

		if (!TileDrawing.IsVisible(tile) || tileData is null)
		{
			return;
		}

		// 油漆
		Texture2D tex = tileDrawing.GetTileDrawTexture(tile, pos.X, pos.Y);

		short tileFrameX = tile.frameX;
		short tileFrameY = tile.frameY;

		// 用于风速、推力等一系列判定的物块坐标，通常来说是挂在墙上的那一格（这边是origin格）
		int topTileX = topLeft.X + tileData.Origin.X;
		int topTileY = topLeft.Y + tileData.Origin.Y;
		int sizeX = tileData.Width;
		int sizeY = tileData.Height;

		int offsetY = tileData.DrawYOffset;

		// 锤子是这样的
		if (WorldGen.IsBelowANonHammeredPlatform(topTileX, topTileY))
		{
			offsetY -= 8;
		}

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(topLeft.X, topLeft.Y, sizeX, sizeY))
		{
			windCycle = tileDrawing.GetWindCycle(topTileX, topTileY, tileDrawing._sunflowerWindCounter);
		}

		// 普通源码罢了
		int totalPushTime = 60;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(topTileX, topTileY, sizeX, sizeY, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;

		// 适配发光涂料
		Rectangle rectangle = new Rectangle(tileFrameX, tileFrameY, 16, 16);
		Color tileLight = Lighting.GetColor(pos);
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(pos.X, pos.Y, tile, tile.type, tileFrameX, tileFrameY, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(pos.Y, pos.X, tile, tile.type, tileFrameX, tileFrameY, tileLight);

		// 基础的坐标
		Vector2 center = new Vector2(topTileX, topTileY).ToWorldCoordinates(autoAddY: 0) - screenPosition;
		Vector2 offset = new Vector2(0f, offsetY);
		center += offset;

		// heightStrength是用于旗帜类物块的，根据高度来决定该格物块的摇曳力度
		float heightStrength = (pos.Y - topLeft.Y + 1) / (float)sizeY;
		if (heightStrength == 0f)
		{
			heightStrength = 0.1f;
		}

		// 计算绘制坐标和origin，原版代码
		Vector2 tileCoordPos = pos.ToWorldCoordinates(0, 0) - screenPosition;
		tileCoordPos += offset;

		// 用于旗帜
		float swayCorrection = Math.Abs(windCycle) * swayOffset * heightStrength;
		Vector2 finalOrigin = center - tileCoordPos;
		Vector2 finalDrawPos = center + new Vector2(0, swayCorrection);

		// 旋转角度
		if (swayOffset == 0f)
		{
			heightStrength = 1f;
		}

		float rotation = -windCycle * swayStrength * heightStrength;

		// 绘制
		spriteBatch.Draw(tex, finalDrawPos, rectangle, tileLight, rotation, finalOrigin, 1f, SpriteEffects.None, 0f);

		// 有火的话绘制火
		if (TileLoader.GetTile(tile.type) is not ITileFlameData tileFlame)
		{
			return;
		}

		// TileDrawing.TileFlameData tileFlameData = tileFlame.GetTileFlameData(pos.X, pos.Y, tile.type, tileFrameY);
		// ulong seed = tileFlameData.flameSeed is 0 ? Main.TileFrameSeed ^ (ulong)(((long)pos.X << 32) | (uint)pos.Y) : tileFlameData.flameSeed;
		// for (int k = 0; k < tileFlameData.flameCount; k++)
		// {
		// float x = Utils.RandomInt(ref seed, tileFlameData.flameRangeXMin, tileFlameData.flameRangeXMax) * tileFlameData.flameRangeMultX;
		// float y = Utils.RandomInt(ref seed, tileFlameData.flameRangeYMin, tileFlameData.flameRangeYMax) * tileFlameData.flameRangeMultY;
		// Main.spriteBatch.Draw(tileFlameData.flameTexture, finalDrawPos + new Vector2(x, y), rectangle, tileFlameData.flameColor, rotation, finalOrigin, 1f, SpriteEffects.None, 0f);
		// }
	}

	public static void MultiTileGrassFluentDraw(Vector2 screenPosition, TileDrawing tileDrawing, SpriteBatch spriteBatch, Point topLeft, Texture2D glowmask = null)
	{
		var tileTopLeft = Main.tile[topLeft];
		var tileData = TileObjectData.GetTileData(tileTopLeft.type, 0);

		if (tileData is null)
		{
			return;
		}

		int bottomTileX = topLeft.X + tileData.Origin.X;
		int bottomTileY = topLeft.Y + tileData.Origin.Y;
		int sizeX = tileData.Width;
		int sizeY = tileData.Height;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(topLeft.X, topLeft.Y, sizeX, sizeY))
		{
			windCycle = tileDrawing.GetWindCycle(bottomTileX, bottomTileY, tileDrawing._sunflowerWindCounter);
		}

		// 原版灌木并不考虑推力
		// int totalPushTime = 60;
		// float pushForcePerFrame = 1.26f;
		// float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(topLeft.X, topLeft.Y, sizeX, sizeY, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		// windCycle += highestWindGridPushComplex;
		Vector2 center = topLeft.ToWorldCoordinates(16f * sizeX * 0.5f, 16f * sizeY) - screenPosition;
		float num = 0.15f;
		ushort type = Main.tile[topLeft].type;

		for (int i = topLeft.X; i < topLeft.X + sizeX; i++)
		{
			for (int j = topLeft.Y; j < topLeft.Y + sizeY; j++)
			{
				Tile tile = Main.tile[i, j];
				if (tile.type != type || !TileDrawing.IsVisible(tile))
				{
					continue;
				}

				short tileFrameX = tile.frameX;
				short tileFrameY = tile.frameY;

				float heightStrength = 1f - (j - topLeft.Y + 1) / (float)sizeY;
				if (heightStrength == 0f)
				{
					heightStrength = 0.1f;
				}

				tileDrawing.GetTileDrawData(i, j, tile, type, ref tileFrameX, ref tileFrameY, out var tileWidth, out var tileHeight, out var tileTop, out var halfBrickHeight, out var addFrX, out var addFrY, out var tileSpriteEffect, out var _, out var _, out var _);

				Color tileLight = Lighting.GetColor(i, j);
				tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(i, j, tile, type, tileFrameX, tileFrameY, ref tileLight, false);
				tileLight = tileDrawing.DrawTiles_GetLightOverride(j, i, tile, type, tileFrameX, tileFrameY, tileLight);

				Vector2 vector2 = new Vector2(i * 16, j * 16 + tileTop) - screenPosition;
				float swayCorrection = Math.Abs(windCycle) * 2f * heightStrength;
				Vector2 origin = center - vector2;
				Texture2D tileDrawTexture = tileDrawing.GetTileDrawTexture(tile, i, j);
				if (tileDrawTexture != null)
				{
					spriteBatch.Draw(tileDrawTexture, center + new Vector2(0f, swayCorrection), new Rectangle(tileFrameX, tileFrameY, tileWidth, tileHeight - halfBrickHeight), tileLight, windCycle * num * heightStrength, origin, 1f, tileSpriteEffect, 0f);
					if (glowmask != null)
					{
						spriteBatch.Draw(glowmask, center + new Vector2(0f, swayCorrection), new Rectangle(tileFrameX, tileFrameY, tileWidth, tileHeight - halfBrickHeight), Color.White, windCycle * num * heightStrength, origin, 1f, tileSpriteEffect, 0f);
					}
				}
			}
		}
	}

	public static bool BedRightClick(int i, int j)
	{
		Player player = Main.LocalPlayer;
		Tile tile = Main.tile[i, j];
		int spawnX = i - tile.TileFrameX / 18 + (tile.TileFrameX >= 72 ? 5 : 2);
		int spawnY = j + 2;

		if (tile.TileFrameY % 38 != 0)
		{
			spawnY--;
		}

		if (!Player.IsHoveringOverABottomSideOfABed(i, j))
		{ // This assumes your bed is 4x2 with 2x2 sections. You have to write your own code here otherwise
			if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
			{
				player.GamepadEnableGrappleCooldown();
				player.sleeping.StartSleeping(player, i, j);
			}
		}
		else
		{
			player.FindSpawn();

			if (player.SpawnX == spawnX && player.SpawnY == spawnY)
			{
				player.RemoveSpawn();
				Main.NewText(Language.GetTextValue("Game.SpawnPointRemoved"), byte.MaxValue, 240, 20);
			}
			else if (Player.CheckSpawn(spawnX, spawnY))
			{
				player.ChangeSpawn(spawnX, spawnY);
				Main.NewText(Language.GetTextValue("Game.SpawnPointSet"), byte.MaxValue, 240, 20);
			}
		}

		return true;
	}

	public static void BedMouseOver<T>(int i, int j)
		where T : ModItem
	{
		Player player = Main.LocalPlayer;

		if (!Player.IsHoveringOverABottomSideOfABed(i, j))
		{
			if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
			{ // Match condition in RightClick. Interaction should only show if clicking it does something
				player.noThrow = 2;
				player.cursorItemIconEnabled = true;
				player.cursorItemIconID = ItemID.SleepingIcon;
			}
		}
		else
		{
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<T>();
		}
	}

	public static bool ChairRightClick(int i, int j)
	{
		Player player = Main.LocalPlayer;

		if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
		{ // Avoid being able to trigger it from long range
			player.GamepadEnableGrappleCooldown();
			player.sitting.SitDown(player, i, j);
		}
		return true;
	}

	public static void ChairMouseOver<T>(int i, int j)
		where T : ModItem
	{
		Player player = Main.LocalPlayer;

		if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
		{
			return;
		}

		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<T>();

		if (Main.tile[i, j].TileFrameX / 18 < 1)
		{
			player.cursorItemIconReversed = true;
		}
	}

	public static bool ChestRightClick(int i, int j)
	{
		Player player = Main.LocalPlayer;
		Tile tile = Main.tile[i, j];
		Main.mouseRightRelease = false;
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

		player.CloseSign();
		player.SetTalkNPC(-1);
		Main.npcChatCornerItem = 0;
		Main.npcChatText = string.Empty;
		if (Main.editChest)
		{
			SoundEngine.PlaySound(SoundID.MenuTick);
			Main.editChest = false;
			Main.npcChatText = string.Empty;
		}

		if (player.editedChestName)
		{
			NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
			player.editedChestName = false;
		}

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			if (left == player.chestX && top == player.chestY && player.chest >= 0)
			{
				player.chest = -1;
				Recipe.FindRecipes();
				SoundEngine.PlaySound(SoundID.MenuClose);
			}
			else
			{
				NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top);
				Main.stackSplit = 600;
			}
		}
		else
		{
			int chest = Chest.FindChest(left, top);
			if (chest >= 0)
			{
				Main.stackSplit = 600;
				if (chest == player.chest)
				{
					player.chest = -1;
					SoundEngine.PlaySound(SoundID.MenuClose);
				}
				else
				{
					player.chest = chest;
					Main.playerInventory = true;
					Main.recBigList = false;
					player.chestX = left;
					player.chestY = top;
					SoundEngine.PlaySound(SoundID.MenuOpen);
				}

				Recipe.FindRecipes();
			}
		}
		return true;
	}

	public static void ChestMouseOver<T>(string chestName, int i, int j)
		where T : ModItem
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
			player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : chestName;
			if (player.cursorItemIconText == chestName)
			{
				player.cursorItemIconID = ModContent.ItemType<T>();

				player.cursorItemIconText = string.Empty;
			}
		}

		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
	}

	public static void ChestMouseFar<T>(string name, int i, int j)
		where T : ModItem
	{
		ChestMouseOver<T>(name, i, j);
		Player player = Main.LocalPlayer;
		if (player.cursorItemIconText == string.Empty)
		{
			player.cursorItemIconEnabled = false;
			player.cursorItemIconID = ItemID.None;
		}
	}

	public static bool ClockRightClick()
	{
		string text = "AM";
		double time = Main.time;
		if (!Main.dayTime)
		{
			time += 54000.0;
		}

		time = time / 86400.0 * 24.0;
		time = time - 7.5 - 12.0;
		if (time < 0.0)
		{
			time += 24.0;
		}

		if (time >= 12.0)
		{
			text = "PM";
		}

		int intTime = (int)time;
		double deltaTime = time - intTime;
		deltaTime = (int)(deltaTime * 60.0);
		string text2 = string.Concat(deltaTime);
		if (deltaTime < 10.0)
		{
			text2 = "0" + text2;
		}

		if (intTime > 12)
		{
			intTime -= 12;
		}

		if (intTime == 0)
		{
			intTime = 12;
		}

		Main.NewText($"Time: {intTime}:{text2} {text}", 255, 240, 20);
		return true;
	}

	public static bool DresserRightClick()
	{
		Player player = Main.LocalPlayer;
		if (Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameY == 0)
		{
			Main.CancelClothesWindow(true);
			Main.mouseRightRelease = false;
			int left = Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameX / 18;
			left %= 3;
			left = Player.tileTargetX - left;
			int top = Player.tileTargetY - Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameY / 18;
			if (player.sign > -1)
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = string.Empty;
			}
			if (Main.editChest)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = string.Empty;
			}
			if (player.editedChestName)
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
				player.editedChestName = false;
			}
			if (Main.netMode == NetmodeID.Server)
			{
				if (left == player.chestX && top == player.chestY && player.chest != -1)
				{
					player.chest = -1;
					Recipe.FindRecipes();
					SoundEngine.PlaySound(SoundID.MenuClose);
				}
				else
				{
					NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top, 0f, 0f, 0, 0, 0);
					Main.stackSplit = 600;
				}
			}
			else
			{
				player.piggyBankProjTracker.Clear();
				int num213 = Chest.FindChest(left, top);
				if (num213 != -1)
				{
					Main.stackSplit = 600;
					if (num213 == player.chest)
					{
						player.chest = -1;
						Recipe.FindRecipes();
						SoundEngine.PlaySound(SoundID.MenuClose);
					}
					else if (num213 != player.chest && player.chest == -1)
					{
						player.chest = num213;
						Main.playerInventory = true;
						Main.recBigList = false;
						SoundEngine.PlaySound(SoundID.MenuOpen);
						player.chestX = left;
						player.chestY = top;
					}
					else
					{
						player.chest = num213;
						Main.playerInventory = true;
						Main.recBigList = false;
						SoundEngine.PlaySound(SoundID.MenuTick);
						player.chestX = left;
						player.chestY = top;
					}
					Recipe.FindRecipes();
					return true;
				}
			}
			return false;
		}
		Main.playerInventory = false;
		player.chest = -1;
		Recipe.FindRecipes(false);
		Main.interactedDresserTopLeftX = Player.tileTargetX;
		Main.interactedDresserTopLeftY = Player.tileTargetY;
		Main.OpenClothesWindow();
		return true;
	}

	public static void DresserMouseFar<T>(string chestName)
		where T : ModItem
	{
		Player player = Main.LocalPlayer;
		Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
		int left = Player.tileTargetX;
		int top = Player.tileTargetY;
		left -= tile.TileFrameX % 54 / 18;
		if (tile.TileFrameY % 36 != 0)
		{
			top--;
		}

		int chestIndex = Chest.FindChest(left, top);
		player.cursorItemIconID = -1;
		if (chestIndex < 0)
		{
			player.cursorItemIconText = Language.GetTextValue("LegacyDresserType.0");
		}
		else
		{
			string defaultName = TileLoader.DefaultContainerName(tile.TileType, left, top); // This gets the ContainerName text for the currently selected language
			if (player.cursorItemIconText == defaultName)
			{
				player.cursorItemIconText = Main.chest[chestIndex].name;
			}
			else
			{
				player.cursorItemIconText = chestName;
			}
			if (player.cursorItemIconText == chestName)
			{
				player.cursorItemIconID = ModContent.ItemType<T>();
				player.cursorItemIconText = string.Empty;
			}
		}
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		if (player.cursorItemIconText == string.Empty)
		{
			player.cursorItemIconEnabled = false;
			player.cursorItemIconID = ItemID.None;
		}
	}

	public static void DresserMouseOver<T>(string chestName)
		where T : ModItem
	{
		Player player = Main.LocalPlayer;
		Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
		int left = Player.tileTargetX;
		int top = Player.tileTargetY;
		left -= tile.TileFrameX % 54 / 18;
		if (tile.TileFrameY % 36 != 0)
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
			if (Main.chest[chest].name != string.Empty)
			{
				player.cursorItemIconText = Main.chest[chest].name;
			}
			else
			{
				player.cursorItemIconText = chestName;
			}
			if (player.cursorItemIconText == chestName)
			{
				player.cursorItemIconID = ModContent.ItemType<T>();
				player.cursorItemIconText = string.Empty;
			}
		}
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		if (Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameY > 0)
		{
			player.cursorItemIconID = ItemID.FamiliarShirt;
			player.cursorItemIconText = "  ";
		}
	}

	public static bool SofaRightClick(int i, int j)
	{
		Player player = Main.LocalPlayer;

		if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
		{ // Avoid being able to trigger it from long range
			player.GamepadEnableGrappleCooldown();
			player.sitting.SitDown(player, i, j);
		}

		return true;
	}

	public static void SofaMouseOver<T>(int i, int j)
		where T : ModItem
	{
		Player player = Main.LocalPlayer;

		if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
		{
			return;
		}

		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<T>();

		if (Main.tile[i, j].TileFrameX / 18 < 1)
		{
			player.cursorItemIconReversed = true;
		}
	}

	/// <summary>
	/// 触发电线信号(正常的StyleHorizontal物块)
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="type"></param>
	/// <param name="tileX"></param>物块组合体横向占多少块
	/// <param name="tileY"></param>物块组合体纵向占多少块
	/// <param name="coordinateX"></param>物块组合体横向每一帧宽度(单位像素)
	/// <param name="coordinateY"></param>物块组合体纵向每一帧高度(单位像素)
	public static void LightHitwire(int i, int j, int type, int tileX, int tileY, int coordinateX = 18, int coordinateY = 18)
	{
		Tile tile = Main.tile[i, j];
		int x = i - tile.TileFrameX / coordinateX % tileX;
		int y = j - tile.TileFrameY / coordinateY % tileY;
		for (int m = x; m < x + tileX; m++)
		{
			for (int n = y; n < y + tileY; n++)
			{
				tile = Main.tile[m, n];
				if (!tile.HasTile)
				{
					continue;
				}

				if (tile.TileType == type)
				{
					if (tile.TileFrameX < coordinateX * tileX)
					{
						tile = Main.tile[m, n];
						tile.TileFrameX += (short)(coordinateX * tileX);
					}
					else
					{
						tile = Main.tile[m, n];
						tile.TileFrameX -= (short)(coordinateX * tileX);
					}
				}
			}
		}
		if (!Wiring.running)
		{
			return;
		}

		for (int k = 0; k < tileX; k++)
		{
			for (int l = 0; l < tileY; l++)
			{
				// 安全化处理
				if (x + k > 0 && x + k < Main.maxTilesX)
				{
					if (y + l > 0 && y + l < Main.maxTilesY)
					{
						// 异形MultiTile检测
						if (Main.tile[x + k, y + l].TileType == type)
						{
							Wiring.SkipWire(x + k, y + l);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 触发电线信号(特例StyleVertical物块)
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="type"></param>
	/// <param name="tileX"></param>物块组合体横向占多少块
	/// <param name="tileY"></param>物块组合体纵向占多少块
	/// <param name="coordinateX"></param>物块组合体横向每一帧宽度(单位像素)
	/// <param name="coordinateY"></param>物块组合体纵向每一帧高度(单位像素)
	public static void LightHitwireStyleVertical(int i, int j, int type, int tileX, int tileY, int coordinateX = 18, int coordinateY = 18)
	{
		Tile tile = Main.tile[i, j];
		int x = i - tile.TileFrameX / coordinateX % tileX;
		int y = j - tile.TileFrameY / coordinateY % tileY;
		for (int m = x; m < x + tileX; m++)
		{
			for (int n = y; n < y + tileY; n++)
			{
				tile = Main.tile[m, n];
				if (!tile.HasTile)
				{
					continue;
				}

				if (tile.TileType == type)
				{
					if (tile.TileFrameY < coordinateY * tileY)
					{
						tile = Main.tile[m, n];
						tile.TileFrameY += (short)(coordinateY * tileY);
					}
					else
					{
						tile = Main.tile[m, n];
						tile.TileFrameY -= (short)(coordinateY * tileY);
					}
				}
			}
		}
		if (!Wiring.running)
		{
			return;
		}

		for (int k = 0; k < tileX; k++)
		{
			for (int l = 0; l < tileY; l++)
			{
				// 安全化处理
				if (x + k > 0 && x + k < Main.maxTilesX)
				{
					if (y + l > 0 && y + l < Main.maxTilesY)
					{
						// 异形MultiTile检测
						if (Main.tile[x + k, y + l].TileType == type)
						{
							Wiring.SkipWire(x + k, y + l);
						}
					}
				}
			}
		}
	}
}
