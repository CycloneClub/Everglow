using Everglow.Commons.DataStructures;
using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.GameContent.Drawing;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles;

public class KelpMoss : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = false;
		TileID.Sets.IsVine[Type] = true;
		TileID.Sets.VineThreads[Type] = true;
		DustType = ModContent.DustType<KelpMoss_dust>();
		Main.tileCut[Type] = true;
		AddMapEntry(new Color(36, 86, 19));
		HitSound = SoundID.Grass;
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield break;
	}

	public override void RandomUpdate(int i, int j)
	{
		int deltaY = 0;
		while (Main.tile[i, j - 1 - deltaY].TileType == Type)
		{
			deltaY++;
			if (deltaY > j - 1)
			{
				break;
			}
		}
		if (deltaY > 15 + Math.Sin(i + j) * 3)
		{
			return;
		}
		if (Main.rand.NextBool(Math.Max(1, deltaY * deltaY - 40)))
		{
			var tileBelow = Main.tile[i, j + 1];
			if (!tileBelow.HasTile)
			{
				tileBelow.TileType = Type;
				tileBelow.HasTile = true;

				// 与原版一致，漆与涂料继承
				tileBelow.CopyPaintAndCoating(Main.tile[i, j]);

				// frame调整与联机同步
				WorldGen.SquareTileFrame(i, j + 1);
				if (Main.netMode is NetmodeID.Server)
				{
					NetMessage.SendTileSquare(-1, i, j + 1);
				}
			}
		}
		base.RandomUpdate(i, j);
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.tile[i, j - 1].TileType != Type)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		DrawAlgae(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// Vanilla function.
	/// </summary>
	/// <param name="tileDrawing"></param>
	/// <param name="screenPosition"></param>
	/// <param name="offSet"></param>
	/// <param name="x"></param>
	/// <param name="startY"></param>
	private void DrawAlgae(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		int maxCount = 40;
		Vector2 lastSegmentPos = drawCenterPos - new Vector2(0, 24);
		float lastRot = 0;
		int xStyle = TileUtils.GetFixedRandomNumber_SingleSeed(tilePos.X, 3);
		Rectangle lastFrame = new Rectangle(xStyle * 34, 0, 32, 16);
		bool tail = false;
		Texture2D dropTex = ModAsset.KelpMoss.Value;
		float displacement = 0;
		for (int t = 0; t < maxCount; t++)
		{
			var tile = TileUtils.SafeGetTile(tilePos + new Point(0, t));
			if (tile.TileType != Type)
			{
				break;
			}
			int paint = tile.TileColor;
			Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.KelpMoss_Path, Type, 1, paint, tileDrawing);
			tex ??= ModAsset.KelpMoss.Value;
			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y + t, tileDrawing._sunflowerWindCounter);
			}

			int totalPushTime = 140;
			float pushForcePerFrame = 0.96f;
			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y + t, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex;
			float rotation = -windCycle * 0.21f + displacement * 0.015f;
			var tileLight = Lighting.GetColor(tilePos + new Point(0, t));
			tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y - t, tile, Type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
			tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y - t, tile, Type, 0, 0, tileLight);
			var origin = new Vector2(16, 0);
			spriteBatch.Draw(tex, lastSegmentPos, lastFrame, tileLight, rotation, origin, 1f, SpriteEffects.None, 0);
			lastRot = rotation;
			Vector2 bone = new Vector2(0, 16).RotatedBy(lastRot);
			lastSegmentPos += bone;
			displacement += bone.X;
			var tileBelow4 = TileUtils.SafeGetTile(tilePos + new Point(0, t + 4));
			if ((!tileBelow4.HasTile || tileBelow4.TileType != Type) && !tail)
			{
				lastFrame.Y = 136;
				for (int j = 1; j <= 3; j++)
				{
					var tileBelow_j = TileUtils.SafeGetTile(tilePos + new Point(0, t + j));
					if (!tileBelow_j.HasTile || tileBelow_j.TileType != Type)
					{
						lastFrame.Y += (4 - j) * 16;
						break;
					}
				}
				tail = true;
			}
			if (tail)
			{
				lastFrame.Y += 16;
			}
			else
			{
				if (lastFrame.Y % 34 == 0)
				{
					lastFrame.Y += 16;
				}
				else
				{
					lastFrame.Y = TileUtils.GetFixedRandomNumber(t + tilePos.Y, tilePos.X, 3) * 34 + 34;
				}
			}
		}
		Vector2 dropOffset = new Vector2(0);
		switch (xStyle)
		{
			case 0:
				dropOffset = new Vector2(3, 0);
				break;
			case 1:
				dropOffset = new Vector2(1, 0);
				break;
			case 2:
				dropOffset = new Vector2(-1, 0);
				break;
		}

		Vector2 worldPos_drop = lastSegmentPos + Main.screenPosition + dropOffset.RotatedBy(lastRot);
		float dropValue = (TileUtils.GetFixedRandomNumber(tilePos, 600) + (int)Main.time) % 600 - 300;
		if (dropValue < 0)
		{
			dropValue = 0;
		}
		if (dropValue == 299 && !Main.gamePaused)
		{
			Dust.NewDustPerfect(worldPos_drop, ModContent.DustType<KelpWaterDrop>(), Vector2.zeroVector);
		}
		dropValue *= 2.5f / 600f;
		var dropLight = Lighting.GetColor(worldPos_drop.ToTileCoordinates()) * dropValue;
		spriteBatch.Draw(dropTex, lastSegmentPos + dropOffset.RotatedBy(lastRot), new Rectangle(0, 206, 2, 2), dropLight, 0, new Vector2(1), 1f, SpriteEffects.None, 0);
	}
}
