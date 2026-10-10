using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.WorldGeneration;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class JadeLakeSargassum : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileCut[Type] = false;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<JadeLakeSargassum_Dust>();

		AddMapEntry(new Color(91, 91, 27));
		HitSound = SoundID.Grass;
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

	public override void RandomUpdate(int i, int j, bool underground)
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
			int length = 0;
			while (TileUtils.SafeGetTile(i, j + length).TileType == tile.TileType)
			{
				length++;
				if (length >= 134)
				{
					break;
				}
			}
			if (length <= 133)
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
		DrawSargassum(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// Draw a piece of lotus
	/// </summary>
	private void DrawSargassum(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
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
			if (!(tileUp.TileType == Type && tileUp.HasTile))
			{
				lastTile = true;
			}
			ushort type = tile.TileType;
			bool joint = (j + tilePos.X) % 3 == 1;
			var frame = new Rectangle(0, 18, 16, 18);
			if (j == 0)
			{
				frame = new Rectangle(0, 54, 16, 16);
			}
			if (joint)
			{
				frame = new Rectangle(0, 36, 16, 16);
			}
			if (lastTile)
			{
				frame = new Rectangle(0, 0, 16, 18);
			}

			// 回声涂料
			if (!TileDrawing.IsVisible(tile))
			{
				continue;
			}

			int paint = Main.tile[tilePos].TileColor;
			Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.JadeLakeSargassum_Path, type, 1, paint, tileDrawing);
			tex ??= ModAsset.JadeLakeSargassum.Value;

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

			var origin = new Vector2(8, 18);
			if (lastTile)
			{
				origin = new Vector2(8, 18);
			}

			var drawPos = drawCenterPos + lastOffset;
			var tileSpriteEffect = SpriteEffects.None;
			spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
			if (lastTile)
			{
				spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
			}

			float size = MathF.Sin((j / (float)height) * MathHelper.Pi);

			// Leaves
			if (joint)
			{
				// Right Leaf
				List<Vertex2D> leaf = new List<Vertex2D>();
				Vector2 leafVel = new Vector2(3, 0) * size;
				Vector2 leafPos = new Vector2(0, -8);
				for (int t = 0; t < 24; t++)
				{
					Vector2 leafWidth = leafVel.RotatedBy(MathHelper.PiOver2).NormalizeSafe() * 9f * size;
					Color getlight = Lighting.GetColor((drawPos + leafPos + Main.screenPosition).ToTileCoordinates());
					getlight.A = 180;
					leaf.Add(drawPos + leafPos + leafWidth, getlight, new Vector3(t / 23f * 74f / 144f, 126f / 144f, 0));
					leaf.Add(drawPos + leafPos - leafWidth, getlight, new Vector3(t / 23f * 74f / 144f, 144f / 144f, 0));
					leafPos += leafVel;
					leafVel = leafVel.RotatedBy(Math.Sin(MathF.Pow(t / 0.5f, 0.5f) + Main.time / 30 + tilePos.X + tilePos.Y / 7f) * 0.08f);
				}
				if (leaf.Count > 2 && size > 0.35f)
				{
					spriteBatch.GraphicsDevice.Textures[0] = tex;
					spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, leaf.ToArray(), 0, leaf.Count - 2);
				}

				// Left Leaf
				leaf = new List<Vertex2D>();
				leafVel = new Vector2(-3, 0) * size;
				leafPos = new Vector2(0, -8);
				for (int t = 0; t < 24; t++)
				{
					Vector2 leafWidth = leafVel.RotatedBy(MathHelper.PiOver2).NormalizeSafe() * 9f * size;
					Color getlight = Lighting.GetColor((drawPos + leafPos + Main.screenPosition).ToTileCoordinates());
					getlight.A = 180;
					leaf.Add(drawPos + leafPos + leafWidth, getlight, new Vector3(t / 23f * 74f / 144f, 126f / 144f, 0));
					leaf.Add(drawPos + leafPos - leafWidth, getlight, new Vector3(t / 23f * 74f / 144f, 144f / 144f, 0));
					leafPos += leafVel;
					leafVel = leafVel.RotatedBy(Math.Sin(MathF.Pow(t / 0.5f, 0.5f) + Main.time / 30 + tilePos.X + tilePos.Y / 7f) * 0.08f);
				}
				if (leaf.Count > 2 && size > 0.35f)
				{
					spriteBatch.GraphicsDevice.Textures[0] = tex;
					spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, leaf.ToArray(), 0, leaf.Count - 2);
				}
				if (size >= 0.5f)
				{
					// Middle Right Leaf
					leaf = new List<Vertex2D>();
					leafVel = new Vector2(1.6f, -1.2f);
					leafPos = new Vector2(0, -8);
					for (int t = 0; t < 16; t++)
					{
						Vector2 leafWidth = leafVel.RotatedBy(MathHelper.PiOver2).NormalizeSafe() * 6f;
						Color getlight = Lighting.GetColor((drawPos + leafPos + Main.screenPosition).ToTileCoordinates());
						getlight.A = 180;
						leaf.Add(drawPos + leafPos + leafWidth, getlight, new Vector3(t / 15f * 56f / 144f, 86f / 144f, 0));
						leaf.Add(drawPos + leafPos - leafWidth, getlight, new Vector3(t / 15f * 56f / 144f, 104f / 144f, 0));
						leafPos += leafVel;
						leafVel = leafVel.RotatedBy(Math.Sin(t * 0.35f + Main.time / 30 + tilePos.X + tilePos.Y / 7f) * 0.08f);
					}
					if (leaf.Count > 2)
					{
						spriteBatch.GraphicsDevice.Textures[0] = tex;
						spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, leaf.ToArray(), 0, leaf.Count - 2);
					}

					// Middle Left Leaf
					leaf = new List<Vertex2D>();
					leafVel = new Vector2(-1.6f, -1.2f);
					leafPos = new Vector2(0, -8);
					for (int t = 0; t < 16; t++)
					{
						Vector2 leafWidth = leafVel.RotatedBy(MathHelper.PiOver2).NormalizeSafe() * 6f;
						Color getlight = Lighting.GetColor((drawPos + leafPos + Main.screenPosition).ToTileCoordinates());
						getlight.A = 180;
						leaf.Add(drawPos + leafPos + leafWidth, getlight, new Vector3(t / 15f * 56f / 144f, 86f / 144f, 0));
						leaf.Add(drawPos + leafPos - leafWidth, getlight, new Vector3(t / 15f * 56f / 144f, 104f / 144f, 0));
						leafPos += leafVel;
						leafVel = leafVel.RotatedBy(Math.Sin(t * 0.35f + Main.time / 30 + tilePos.X + tilePos.Y / 7f) * 0.08f);
					}
					if (leaf.Count > 2)
					{
						spriteBatch.GraphicsDevice.Textures[0] = tex;
						spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, leaf.ToArray(), 0, leaf.Count - 2);
					}
				}
			}
			lastOffset += new Vector2(0, -16).RotatedBy(rotation);
		}
	}
}
