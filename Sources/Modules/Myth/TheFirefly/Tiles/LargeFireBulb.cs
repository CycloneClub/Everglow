using Everglow.Commons.TileHelper;
using Everglow.Myth.TheFirefly.Dusts;
using Everglow.Myth.TheFirefly.Projectiles;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles;

internal class LargeFireBulbTestItem : ModItem
{
	public override string Texture => "Everglow/" + ModAsset.LargeFireBulb_item_Path;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useAnimation = 15;
		Item.useTime = 10;
		Item.maxStack = Item.CommonMaxStack;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.width = 30;
		Item.height = 30;
	}

	// 在多人模式不可用，不过这个是放置测试物品，就不适配了
	public override bool? UseItem(Player player)
	{
		if (player.ItemAnimationJustStarted)
		{
			var tileCoords = Main.MouseWorld.ToTileCoordinates();
			return LargeFireBulb.PlaceMe(tileCoords.X, tileCoords.Y, 0);
		}
		return true;
	}
}

internal class LargeFireBulb : ModTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoFail[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileCut[Type] = true;

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.CoordinateHeights = new int[4]
		{
			16,
			16,
			16,
			16,
		};

		TileObjectData.newTile.StyleWrapLimit = 111;
		TileObjectData.newTile.DrawYOffset = -6;
		TileObjectData.addTile(Type);
		HitSound = SoundID.Grass;
		DustType = ModContent.DustType<FluorescentTreeDust>();
		AddMapEntry(new Color(28, 132, 255));
	}

	/// <summary>
	/// 要用这个方法来放置LargeFireBulb到指定位置，这样可以保证生成出来就有根、果实上部、果实下部三个部分 <br/>
	/// 注意，不可在多人模式客户端调用
	/// </summary>
	/// <param name="x">根左上角的x坐标</param>
	/// <param name="y">根左上角的y坐标</param>
	/// <param name="stemLength">茎长，花有四个部分: 根、茎、果实上部、果实下部。其中茎可以没有，因此stemLength最小为0</param>
	/// <param name="broadcast">在服务器是否同步物块信息到所有客户端</param>
	/// <returns>放置是否成功</returns>
	public static bool PlaceMe(int x, int y, ushort stemLength = 0, bool broadcast = true)
	{
		if (Main.netMode is NetmodeID.MultiplayerClient)
		{
			return false;
		}

		var modTile = TileLoader.GetTile(ModContent.TileType<LargeFireBulb>()) as LargeFireBulb;

		// 根
		bool succeed = modTile.TryGrow(x, y, broadcast);
		if (!succeed)
		{
			return false;
		}

		// 茎
		int height = 4;
		for (int k = 0; k < stemLength; k++)
		{
			succeed &= modTile.TryGrow(x, y + height, broadcast);
			if (!succeed)
			{
				return false;
			}

			height += 4;
		}

		// 果实1
		succeed &= modTile.TryGrow(x, y + height, broadcast);
		height += 4;
		if (!succeed)
		{
			return false;
		}

		// 果实2
		succeed &= modTile.TryGrow(x, y + height, broadcast);
		return succeed;
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		var tile = Main.tile[i, j];
		int lx = i - (tile.TileFrameX % 36) / 18;
		int ly = j - tile.TileFrameY / 18;

		noBreak = true;

		// 保证当前节结构完整
		for (int m = 0; m < 2; m++)
		{
			for (int n = 0; n < 4; n++)
			{
				noBreak &= HasSameTileAt(lx + m, ly + n);
			}
		}

		// 保证上方有连接物
		noBreak &= Main.tile[lx, ly - 1].HasTile;

		// 爬升到根
		int y = ly;
		while (y > 20 && HasSameTileAt(lx, y - 1))
		{
			y--;
		}

		// 保证至少有三节，否则绘制会出问题
		noBreak &= HasSameTileAt(lx, y + 4) && HasSameTileAt(lx, y + 8) && Main.tile[lx, y - 1].HasTile && Main.tile[lx + 1, y - 1].HasTile;

		if (!noBreak)
		{
			if (tile.TileFrameX == 180 && tile.TileFrameY == 0)
			{
				GenerateProjectile(i, j);
			}
		}
		// tile.TileFrameX %= 36;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override void RandomUpdate(int i, int j, bool underground)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX % 36 != 0)
		{
			i -= 1;
		}
		if (!Main.rand.NextBool(15))
		{
			return;
		}
		var keyCoord = new Point(i, j + GetSameTileBelow(i, j) - 4);
		var keyTile = Main.tile[keyCoord];
		if (keyTile.TileFrameX < 180)
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 4; y++)
				{
					var updateTile = Main.tile[keyCoord + new Point(x, y)];
					updateTile.TileFrameX += 36;
				}
			}
		}
		else
		{
			TryGrow(i, keyCoord.Y + 4);
		}

		base.RandomUpdate(i, j, underground);
	}

	private bool TryGrow(int x, int y, bool broadcast = true)
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				var coords = new Point(x + i, y + j);
				var tile = Main.tile[coords];
				if (tile.HasTile)
				{
					return false;
				}
			}
		}

		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				var coords = new Point(x + i, y + j);
				var tile = Main.tile[coords];
				tile.TileType = Type;
				tile.HasTile = true;
				tile.TileFrameX = (short)(i * 18 + 180);
				tile.TileFrameY = (short)(j * 18);
			}
		}
		if (GetSameTileUpon(x, y) > 4)
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					var coords = new Point(x + i, y + j - 4);
					var tile = Main.tile[coords];
					tile.TileType = Type;
					tile.HasTile = true;
					tile.TileFrameX = (short)(i * 18);
					tile.TileFrameY = (short)(j * 18);
				}
			}
		}
		if (broadcast && Main.netMode is NetmodeID.Server)
		{
			NetMessage.SendTileSquare(-1, x, y, 2, 4);
		}

		return true;
	}

	private bool HasSameTileAt(int i, int j)
	{
		if (!WorldGen.InWorld(i, j, 10))
		{
			return false;
		}

		var tile = Main.tile[i, j];
		return tile != null && tile.HasTile && tile.type == Type;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX % 36 == 0)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		DrawFireBulb(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// Draw a piece of moss
	/// </summary>
	private void DrawFireBulb(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[tilePos];
		ushort type = tile.TileType;
		var frame = new Rectangle(tile.TileFrameX / 36 * 80, 0, 80, 80);
		if (GetSameTileUpon(tilePos.X, tilePos.Y) % 2 != 1)
		{
			return;
		}
		if (GetSameTileUpon(tilePos.X, tilePos.Y) > 1)
		{
			frame = new Rectangle(tile.TileFrameX / 36 * 80 + 20, 48, 40, 40);
		}
		if (GetSameTileBelow(tilePos.X, tilePos.Y) == 4)
		{
			frame = new Rectangle(tile.TileFrameX / 36 * 80, 80, 80, 160);
		}
		if (GetSameTileBelow(tilePos.X, tilePos.Y) < 4)
		{
			return;
		}
		var offset = new Vector2(10, -20);
		var stability = 1f;
		var origin = new Vector2(frame.Width * 0.5f, 0);

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[tilePos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.LargeFireBulb_Path, type, 1, paint, tileDrawing);
		tex ??= ModAsset.LargeFireBulb.Value;

		var finalPoint = tilePos + new Point(0, GetSameTileBelow(tilePos.X, tilePos.Y) - 4);

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(finalPoint.X, finalPoint.Y, 1, 1))
		{
			windCycle = tileDrawing.GetWindCycle(finalPoint.X, finalPoint.Y, tileDrawing._sunflowerWindCounter);
		}

		// 摆长和周期的关系
		int totalPushTime = (int)Math.Sqrt(GetSameTileUpon(finalPoint.X, finalPoint.Y)) * 120;
		float pushForcePerFrame = 0.98f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(finalPoint.X, finalPoint.Y, 4, 4, totalPushTime, pushForcePerFrame, 8, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;

		float rotation = -windCycle * 0.005f * stability;
		offset += (new Vector2(0, 32).RotatedBy(rotation) - new Vector2(0, 32)) * GetSameTileUpon(tilePos.X, tilePos.Y);

		var tileLight = Lighting.GetColor(tilePos);

		// 支持发光涂料
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y, tile, type, 0, 0, tileLight);
		spriteBatch.Draw(tex, drawCenterPos + offset, frame, tileLight, rotation, origin, 1f, SpriteEffects.None, 0f);
	}

	public int GetSameTileUpon(int i, int j)
	{
		for (int y = 0; y < 6000; y++)
		{
			if (j - y < 20)
			{
				return y;
			}
			var tile = Main.tile[i, j - y];
			if (tile.TileType != Type)
			{
				return y;
			}
		}
		return -1;
	}

	public int GetSameTileBelow(int i, int j)
	{
		for (int y = 0; y < 6000; y++)
		{
			if (j - y > Main.maxTilesY - 20)
			{
				return y;
			}
			var tile = Main.tile[i, j + y];
			if (tile.TileType != Type)
			{
				return y;
			}
		}
		return -1;
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		float scaleLight = default;
		var tile = Main.tile[i, j];
		scaleLight = tile.TileFrameX - tile.TileFrameX % 36;
		scaleLight /= 180f;
		if (HasSameTileAt(i, j - 4)) // 上面有同样物块，不是根
		{
			if (!HasSameTileAt(i, j + 4)) // 下面没有同样物块，是果实2
			{
				r = 1 * scaleLight;
				g = 1.6f * scaleLight;
				b = 1.8f * scaleLight;
			}
			else if (!HasSameTileAt(i, j + 8)) // 下面有同样物块，但下面第二格有，是果实1
			{
				r = 0;
				g = 0.4f * scaleLight;
				b = 0.8f * scaleLight;
			}
			else // 下面有同样物块，下面第二格也有，是茎
			{
				r = 0;
				g = 0;
				b = 0;
			}
		}
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX == 180 && tile.TileFrameY == 0)
		{
			GenerateProjectile(i, j);
		}
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		base.KillMultiTile(i, j, frameX, frameY);
	}

	private void GenerateProjectile(int i, int j)
	{
		Projectile.NewProjectile(WorldGen.GetItemSource_FromTileBreak(i, j), new Vector2(i + 1, j + 5) * 16 + new Vector2(0, 8), Vector2.zeroVector, ModContent.ProjectileType<FallenDropFruit>(), 100, 10);
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 1;
		base.NumDust(i, j, fail, ref num);
	}
}
