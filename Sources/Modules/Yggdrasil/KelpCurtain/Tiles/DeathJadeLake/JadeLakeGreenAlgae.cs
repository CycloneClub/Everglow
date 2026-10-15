using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.WorldGeneration;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class JadeLakeGreenAlgae : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<JadeLakeGreenAlgaeDust>();

		AddMapEntry(new Color(33, 96, 24));
		HitSound = SoundID.Grass;
	}

	public override void NearbyEffects(int i, int j, bool closer)
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
		base.NearbyEffects(i, j, closer);
	}

	public override bool CreateDust(int i, int j, ref int type)
	{
		for (int k = 0; k < 2; k++)
		{
			Vector2 pos = new Point(i, j).ToWorldCoordinates();
			Dust d = Dust.NewDustDirect(pos - new Vector2(6, 8) + new Vector2(4), 12, 16, type);
			d.noGravity = true;
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

		if (tile2.TileType != tile.TileType && !tile2.HasTile)
		{
			int length = 0;
			while (TileUtils.SafeGetTile(i, j + length).TileType == tile.TileType)
			{
				length++;
				if (length >= 31)
				{
					break;
				}
			}
			if (length <= 30)
			{
				tile2.TileType = Type;
				tile2.HasTile = true;
			}
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.tile[i, j + 1].TileType != Type)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		else if (j == (Main.screenPosition + new Vector2(0, Main.screenHeight)).ToTileCoordinates().Y)
		{
			for (int y = 1; y < 30; y++)
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
		var lastOffset2 = new Vector2(0, 12);
		int height = 0;
		for (int j = 0; j < 40; j++)
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
		for (int j = 0; j < 40; j++)
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
			if (!(tileUp.TileType == Type && tileUp.HasTile))
			{
				lastTile = true;
			}
			ushort type = tile.TileType;
			int frameY = (tilePos.X + tilePos.Y + j) % 6;
			int frameX = tilePos.X % 3;
			frameY += 1;
			var frame = new Rectangle(frameX * 16, frameY * 18, 16, 18);
			if (j == 0)
			{
				frame = new Rectangle(frameX * 16, 126, 16, 18);
			}
			if (lastTile)
			{
				frame = new Rectangle(frameX * 16, 0, 16, 18);
			}

			// 回声涂料
			if (!TileDrawing.IsVisible(tile))
			{
				continue;
			}

			int paint = Main.tile[tilePos].TileColor;
			Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.JadeLakeGreenAlgae_Path, type, 1, paint, tileDrawing);
			tex ??= ModAsset.JadeLakeGreenAlgae.Value;

			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);
			}

			int totalPushTime = 140;
			float pushForcePerFrame = 0.96f;
			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y - j, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex;
			float rotation = windCycle * 0.21f;
			rotation -= lastOffset.X / (22f + j * 3f);
			rotation += MathF.Sin(j / (float)height * MathHelper.Pi) * MathF.Sin(tilePos.X * 2 + tilePos.Y - j * 0.45f + (float)Main.time / 20f) * 0.15f;
			rotation += MathF.Sin(j / (float)height * MathHelper.Pi) * MathF.Sin(tilePos.X * 2 + tilePos.Y + (float)Main.time / 20f) * 6f / (height + 25);
			float rotation2 = windCycle * 0.21f;
			rotation2 -= lastOffset2.X / (22f + j * 3f);
			rotation2 += MathF.Sin(j / (float)height * MathHelper.Pi) * MathF.Sin((tilePos.X + 3) * 2 + tilePos.Y - (j + 1) * 0.45f + (float)Main.time / 20f) * 0.15f;
			rotation2 += MathF.Sin(j / (float)height * MathHelper.Pi) * MathF.Sin((tilePos.X + 3) * 2 + tilePos.Y + (float)Main.time / 20f) * -6f / (height + 25);
			var tileLight = Lighting.GetColor(tilePos + new Point(0, -j));

			// 支持发光涂料
			tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y - j, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
			tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y - j, tile, type, 0, 0, tileLight);
			if (height > 10 && height - j <= 10)
			{
				tileLight *= (height - j) / 20f + 0.5f;
			}
			var origin = new Vector2(8, 18);
			if (lastTile)
			{
				origin = new Vector2(8, 18);
			}
			int style = 0;
			int offsetX = 0;
			if (tilePos.X % 9 is >= 3 and < 6)
			{
				style = 1;
			}
			if (tilePos.X % 9 >= 6)
			{
				style = 2;
			}
			var drawPos = drawCenterPos + lastOffset + new Vector2(offsetX, 0);
			var tileSpriteEffect = SpriteEffects.None;
			int finalHeight = height / 3 * 2;
			switch (style)
			{
				case 0:
					spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
					break;
				case 1:
					offsetX = -2;
					drawPos = drawCenterPos + lastOffset + new Vector2(offsetX, 0);
					spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
					offsetX = 4;
					frameX++;
					frameX %= 3;
					frame.X = frameX * 16;
					tileLight *= 0.7f;
					tileLight.R += 25;
					tileLight.A = 200;
					drawPos = drawCenterPos + lastOffset2 + new Vector2(offsetX, 0);
					if (j < finalHeight)
					{
						spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation2, origin, 1f, tileSpriteEffect, 0f);
					}
					else if (j == finalHeight)
					{
						frame.Y = 0;
						spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation2, origin, 1f, tileSpriteEffect, 0f);
					}
					break;
				case 2:
					offsetX = 6;
					drawPos = drawCenterPos + lastOffset + new Vector2(offsetX, 0);
					spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
					finalHeight = height / 2;
					offsetX = -6;
					frameX++;
					frameX %= 3;
					frame.X = frameX * 16;
					tileLight *= 0.7f;
					tileLight.R += 15;
					tileLight.A = 180;
					drawPos = drawCenterPos + lastOffset2 + new Vector2(offsetX, 0);
					if (j < finalHeight)
					{
						spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation2, origin, 1f, tileSpriteEffect, 0f);
					}
					else if (j == finalHeight)
					{
						frame.Y = 0;
						spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation2, origin, 1f, tileSpriteEffect, 0f);
					}
					break;
			}

			lastOffset += new Vector2(0, -16).RotatedBy(rotation);
			lastOffset2 += new Vector2(0, -16).RotatedBy(rotation2);
		}
	}
}
