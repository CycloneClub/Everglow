using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class CrimsonMoonAlgea : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileCut[Type] = false;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.TouchDamageImmediate[Type] = 30;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.addTile(Type);

		DustType = ModContent.DustType<JadeLakeRedAlgaeDust>();

		AddMapEntry(new Color(127, 36, 89));
		HitSound = SoundID.Grass;
	}

	public override bool IsTileDangerous(int i, int j, Player player)
	{
		if (player.HasBuff(BuffID.Dangersense))
		{
			return true;
		}
		return false;
	}

	public override void RandomUpdate(int i, int j)
	{
		var tile = Main.tile[i, j];
		if (tile.LiquidAmount <= 0)
		{
			WorldGen.KillTile(i, j);
			return;
		}
		var tile2 = Main.tile[i, j - 1];

		if (tile2.TileType != tile.TileType && !tile2.HasTile && tile2.LiquidAmount > 0)
		{
			tile2.TileType = (ushort)ModContent.TileType<CrimsonMoonAlgea_fruit>();
			tile2.HasTile = true;
		}
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
			Dust d = Dust.NewDustDirect(pos - new Vector2(20, 40) + new Vector2(4), 40, 50, type);
			d.noGravity = true;
		}
		return false;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.tile[i, j + 1].TileType != Type)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		else if (j == (Main.screenPosition + new Vector2(0, Main.screenHeight)).ToTileCoordinates().Y)
		{
			for (int y = 1; y < 133; y++)
			{
				Tile stepDown = TileUtils.SafeGetTile(i, j + y);
				if (stepDown.TileType != Type)
				{
					TileFluentDrawManager.AddFluentPoint(this, i, j + y - 1);
					break;
				}
			}
		}
		return false;
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
		var lastOffset = new Vector2(0, 12);
		int height = 0;
		for (int j = 0; j < 133; j++)
		{
			height++;
			if (tilePos.Y - j < 21)
			{
				break;
			}
			var tile = Main.tile[tilePos + new Point(0, -j)];
			if (!(tile.TileType == Type && tile.HasTile))
			{
				break;
			}
		}
		for (int j = 0; j < 133; j++)
		{
			if (tilePos.Y - j < 21)
			{
				return;
			}
			var tile = Main.tile[tilePos + new Point(0, -j)];
			if (!(tile.TileType == Type && tile.HasTile))
			{
				return;
			}
			var tileUp = Main.tile[tilePos + new Point(0, -j - 1)];
			bool lastTile = false;
			bool lastTileHasFruit = false;
			if (!(tileUp.TileType == Type && tileUp.HasTile))
			{
				lastTile = true;
				if (tileUp.TileType == ModContent.TileType<CrimsonMoonAlgea_fruit>())
				{
					lastTileHasFruit = true;
				}
			}
			ushort type = tile.TileType;
			bool joint = (j + tilePos.X) % 2 == 1;
			int style = TileUtils.GetFixedRandomNumber(tilePos.X, j + tilePos.Y) % 3;
			var frame = new Rectangle(14, 132 + 34 * style, 36, 32);
			if (lastTile)
			{
				if (lastTileHasFruit)
				{
					frame = new Rectangle(64 * style, 66, 64, 64);
				}
				else
				{
					frame = new Rectangle(64 * style, 0, 64, 64);
				}
			}

			// 回声涂料
			if (!TileDrawing.IsVisible(tile))
			{
				continue;
			}

			int paint = Main.tile[tilePos].TileColor;
			Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.CrimsonMoonAlgea_Path, type, 1, paint, tileDrawing);
			tex ??= ModAsset.CrimsonMoonAlgea.Value;

			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);
			}

			int totalPushTime = 140;
			float pushForcePerFrame = 0.96f;
			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y - j, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex;
			float rotation = windCycle * 0.12f;
			rotation -= lastOffset.X / (22f + j * 3f);
			rotation += MathF.Sin(j / (float)height * MathHelper.Pi) * MathF.Sin(tilePos.X * 2 + tilePos.Y - j * 0.45f + (float)Main.time / 20f) * 0.15f;
			rotation += MathF.Sin(j / (float)height * MathHelper.Pi) * MathF.Sin(tilePos.X * 2 + tilePos.Y + (float)Main.time / 20f) * 6f / (height + 25);
			var tileLight = Lighting.GetColor(tilePos + new Point(0, -j));

			// 支持发光涂料
			tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y - j, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
			tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y - j, tile, type, 0, 0, tileLight);

			var origin = new Vector2(frame.Width * 0.5f, frame.Height);

			var drawPos = drawCenterPos + lastOffset;
			var tileSpriteEffect = SpriteEffects.None;

			if (joint || j == 0 || lastTile)
			{
				spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
			}
			if (lastTile)
			{
				frame.X += 192;
				spriteBatch.Draw(tex, drawPos, frame, new Color(0.1f, 0.1f, 0.1f, 0), rotation, origin, 1f, tileSpriteEffect, 0f);
				if (lastTileHasFruit)
				{
					Lighting.AddLight(drawPos + new Vector2(0, -40).RotatedBy(rotation) + Main.screenPosition, new Vector3(1.5f, 1.3f, 1f) * (1.5f + MathF.Sin(Main.GlobalTimeWrappedHourly + TileUtils.GetFixedRandomNumber_SingleSeed(tilePos.GetHashCode())) * 0.5f));
				}
			}
			if (j == 0)
			{
				frame = new Rectangle(94, 244, 54, 16);
				origin = new Vector2(frame.Width * 0.5f, frame.Height);
				spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
			}
			lastOffset += new Vector2(0, -16).RotatedBy(rotation);
		}
	}
}
