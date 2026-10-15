using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;

public class IslePeachTree_medium : ModTile, ITileFluentlyDrawn
{
	public const int MaxLength = 6;

	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileCut[Type] = false;
		Main.tileLavaDeath[Type] = true;
		Main.tileAxe[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<IslePeachTree_Sawdust>();

		AddMapEntry(new Color(205, 101, 147));
		HitSound = SoundID.Dig;
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		Tile bottomTile = TileUtils.SafeGetTile(i, j + 1);
		Tile tile = Main.tile[i, j];
		if (tile.HasTile && tile.TileType == Type)
		{
			if (!bottomTile.HasTile || (bottomTile.TileType != Type && !Main.tileSolid[bottomTile.type] && !Main.tileSolidTop[bottomTile.type]))
			{
				int deltaY = 0;
				while (true)
				{
					Tile topTile = TileUtils.SafeGetTile(i, j - deltaY);
					if (topTile.HasTile && topTile.TileType == Type && j - deltaY > 0)
					{
						WorldGen.KillTile(i, j - deltaY, false, false, true);
					}
					else
					{
						break;
					}
					deltaY++;
				}
			}
		}
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override bool CreateDust(int i, int j, ref int type)
	{
		for (int k = 0; k < 2; k++)
		{
			Vector2 pos = new Point(i, j).ToWorldCoordinates();
			var d = Dust.NewDustDirect(pos - new Vector2(20, 40) + new Vector2(4), 40, 50, type);
			d.noGravity = true;
		}
		return false;
	}

	public override void RandomUpdate(int i, int j)
	{
		var tile = Main.tile[i, j];
		var tile2 = Main.tile[i, j - 1];

		if (tile2.TileType != tile.TileType && !tile2.HasTile)
		{
			int length = 0;
			int maxLengthHere = MaxLength - TileUtils.GetFixedRandomNumber(tile) % 3;
			while (TileUtils.SafeGetTile(i, j + length).TileType == Type)
			{
				length++;
				if (length >= maxLengthHere + 1)
				{
					break;
				}
			}
			if (length <= maxLengthHere)
			{
				tile2.TileType = Type;
				tile2.HasTile = true;
			}
		}

		int topY = j;
		for (int y = 0; y < 12; y++)
		{
			if (TileUtils.SafeGetTile(i, j - y).TileType != Type)
			{
				topY = j - y + 1;
				break;
			}
		}
		var petal = new PeachBlossom
		{
			Velocity = new Vector2(0, 0.5f).RotatedByRandom(Math.PI * 2),
			Active = true,
			Visible = true,
			Position = new Vector2(i, topY).ToWorldCoordinates() + new Vector2(0, Main.rand.NextFloat()).RotatedByRandom(MathHelper.TwoPi) * 120 + new Vector2(0, -60),
			MaxTime = 3600,
			Scale = Main.rand.NextFloat(1f, 1.5f),
			Frame = Main.rand.Next(10),
			ai = new float[] { Main.rand.NextFloat(1f, 8f), -1 },
		};
		Ins.VFXManager.Add(petal);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		TileFluentDrawManager.AddFluentPoint(this, i, j);
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		DrawPeachTree(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// Draw a piece of peach blossom
	/// </summary>
	private void DrawPeachTree(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		int toBottom = 0;
		int toTop = 0;
		var tile = TileUtils.SafeGetTile(tilePos);
		for (int j = 1; j < MaxLength; j++)
		{
			toBottom = j;
			var checkTile = TileUtils.SafeGetTile(tilePos.X, tilePos.Y + j);
			if (checkTile.type != Type)
			{
				break;
			}
		}
		for (int j = 1; j < MaxLength; j++)
		{
			toTop = j;
			var checkTile = TileUtils.SafeGetTile(tilePos.X, tilePos.Y - j);
			if (checkTile.type != Type)
			{
				break;
			}
		}
		Texture2D tex = ModAsset.IslePeachTree_medium.Value;

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[tilePos].TileColor;
		tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.IslePeachTree_medium_Path, Type, 1, paint, tileDrawing);
		tex ??= ModAsset.IslePeachTree_medium.Value;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
		{
			windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter) * 0.25f;
		}

		int totalPushTime = 140;
		float pushForcePerFrame = 0.96f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		int styleTop = TileUtils.GetFixedRandomNumber_SingleSeed(tilePos.X, 2);
		if (toTop == 1)
		{
			highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X - 3, tilePos.Y - 7, 7, 7, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		}
		windCycle += highestWindGridPushComplex * 0.25f;
		float rotation = windCycle * 0.1f;
		if (!Main.gamePaused)
		{
			if (windCycle > 0.1f && toTop == 1)
			{
				if (Main.rand.NextBool(2))
				{
					var petal = new PeachBlossom
					{
						Velocity = new Vector2(0, 0.5f).RotatedByRandom(Math.PI * 2),
						Active = true,
						Visible = true,
						Position = tilePos.ToWorldCoordinates() + new Vector2(0, Main.rand.NextFloat()).RotatedByRandom(MathHelper.TwoPi) * new Vector2(1, 0.5f) * 150 + new Vector2(0, -80),
						MaxTime = 3600,
						Scale = Main.rand.NextFloat(0.8f, 1.2f),
						Frame = Main.rand.Next(10),
						ai = new float[] { Main.rand.NextFloat(1f, 8f), -1 },
					};
					if (styleTop == 1)
					{
						petal.Position = tilePos.ToWorldCoordinates() + new Vector2(0, Main.rand.NextFloat()).RotatedByRandom(MathHelper.TwoPi) * 100 + new Vector2(0, -100);
					}
					Ins.VFXManager.Add(petal);
				}
			}
		}

		var tileLight = Lighting.GetColor(tilePos);

		// 支持发光涂料
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, Type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y, tile, Type, 0, 0, tileLight);

		if (toTop == 1)
		{
			var frame = new Rectangle(14, 4, 340, 212);
			var origin = new Vector2(170, 224);
			switch (styleTop)
			{
				case 0:
					frame = new Rectangle(14, 4, 340, 212);
					origin = new Vector2(167, 212);
					break;
				case 1:
					frame = new Rectangle(370, 38, 240, 220);
					origin = new Vector2(114, 220);
					break;
			}
			TileUtils.VertexDraw_Grid(drawCenterPos + new Vector2(0, 2), frame, origin, tex, spriteBatch, rotation);
		}
		if (toTop > 1 && toBottom > 1)
		{
			int style = TileUtils.GetFixedRandomNumber(tile) % 8;
			var frame = new Rectangle(0, 236 + 18 * style, 34, 16);
			var origin = new Vector2(15, 16);
			TileUtils.VertexDraw_4_Corner(drawCenterPos, frame, origin, tex, spriteBatch, rotation);
		}
		if (toBottom == 1)
		{
			var frame = new Rectangle(50, 340, 38, 40);
			var origin = new Vector2(19, 32);
			TileUtils.VertexDraw_Grid(drawCenterPos + new Vector2(0, 12), frame, origin, tex, spriteBatch, rotation);
		}
	}
}
