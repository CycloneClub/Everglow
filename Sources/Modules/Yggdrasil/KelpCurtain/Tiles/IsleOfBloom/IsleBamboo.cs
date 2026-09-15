using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;

public class IsleBamboo : ModTile, ITileFluentlyDrawn
{
	public const int MaxLength = 60;

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
		DustType = ModContent.DustType<IsleBamboo_Sawdust>();

		AddMapEntry(new Color(55, 91, 40));
		HitSound = SoundID.Grass;
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<DryIsleBamboo_Item>());
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
			int maxLengthHere = MaxLength - (TileUtils.GetFixedRandomNumber(tile) % 10);
			while (TileUtils.SafeGetTile(i, j + length).TileType == tile.TileType)
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
		bool canGenerateDust = true;
		for (int y = 0; y < 10; y++)
		{
			if (TileUtils.SafeGetTile(i, j + y).type != Type)
			{
				canGenerateDust = false;
				break;
			}
		}
		if (canGenerateDust)
		{
			var leaf = new BambooLeaf
			{
				Velocity = new Vector2(0, 0.5f).RotatedByRandom(Math.PI * 2),
				Active = true,
				Visible = true,
				Position = new Vector2(i, j).ToWorldCoordinates() + new Vector2(0, Main.rand.NextFloat()).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = 3600,
				Scale = Main.rand.NextFloat(0.5f, 0.7f),
				Frame = Main.rand.Next(8),
				FlipAngle = Main.rand.NextFloat(MathHelper.TwoPi),
				FlipSpeed = Main.rand.NextFloat(-0.4f, 0.4f),
				ai = new float[] { Main.rand.NextFloat(1f, 8f), -1 },
			};
			Ins.VFXManager.Add(leaf);
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.tile[i, j + 1].TileType != Type)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		else if (j == (Main.screenPosition + new Vector2(0, Main.screenHeight / Main.GameViewMatrix.Zoom.Y + Main.offScreenRange + 160)).ToTileCoordinates().Y)
		{
			for (int y = 1; y < MaxLength; y++)
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
		DrawBamboo(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// Draw a piece of lotus
	/// </summary>
	private void DrawBamboo(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var lastOffset = new Vector2(0, 12);
		int height = 0;
		float lastRot = 0;
		for (int j = 0; j < MaxLength; j++)
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
		float natureBendValue = (TileUtils.GetFixedRandomNumber(tilePos.X, tilePos.Y) % 10) - 4.5f;
		natureBendValue += MathF.Sin((float)(Main.time * 0.03f + (TileUtils.GetFixedRandomNumber(tilePos.X, tilePos.Y) % 10) / 9f * MathHelper.PiOver2)) * Math.Clamp(Main.windSpeedCurrent, 0, 1);
		if (height < 20)
		{
			natureBendValue *= 0;
		}
		else if (height < 30)
		{
			natureBendValue *= (height - 20) / 10f;
		}
		Texture2D tex = ModAsset.IsleBamboo.Value;
		List<Vertex2D> bamboo_top = new List<Vertex2D>();
		for (int j = 0; j < MaxLength; j++)
		{
			if (tilePos.Y - j < 21)
			{
				return;
			}
			var tile = Main.tile[tilePos + new Point(0, -j)];
			if (!(tile.TileType == Type && tile.HasTile))
			{
				break;
			}

			int lastTileDis = 0;
			int maxLastTileDis = 24;
			for (int k = 1; k < maxLastTileDis; k++)
			{
				var tileUp = Main.tile[tilePos + new Point(0, -j - k)];
				if (!(tileUp.TileType == Type && tileUp.HasTile))
				{
					lastTileDis = k;
					break;
				}
			}
			ushort type = tile.TileType;
			int jointStyle = 4 - (j + TileUtils.GetFixedRandomNumber_SingleSeed(tilePos.X)) % 5;
			var frame = new Rectangle(3 + 12 * jointStyle, 116, 10, 18);
			if (lastTileDis > 0)
			{
				if (lastTileDis < 20)
				{
					frame.Y -= 20;
				}
				if (lastTileDis < 16)
				{
					frame.Y -= 20;
				}
			}

			// 回声涂料
			if (!TileDrawing.IsVisible(tile))
			{
				continue;
			}

			int paint = Main.tile[tilePos].TileColor;
			tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.IsleBamboo_Path, type, 1, paint, tileDrawing);
			tex ??= ModAsset.IsleBamboo.Value;

			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y - j, 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y - j, tileDrawing._sunflowerWindCounter) * 0.25f;
			}

			int totalPushTime = 140;
			float pushForcePerFrame = 0.96f;
			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y - j, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex * 0.25f;
			float rotation = windCycle * 0.01f + natureBendValue / 540f;
			if (lastTileDis > 0)
			{
				rotation *= MathF.Pow(maxLastTileDis - lastTileDis, 2) / 6f;
			}
			float addRot = rotation - lastRot;
			if (Math.Abs(rotation) > MathHelper.PiOver2 * 0.8)
			{
				float rotDecay = MathHelper.PiOver2 * 0.8f - Math.Abs(rotation);
				addRot *= MathF.Pow(10, rotDecay);
			}
			rotation = lastRot + addRot;

			// rotation -= lastOffset.X / (22f + j * 3f);
			// rotation += MathF.Sin(j / (float)height * MathHelper.Pi) * MathF.Sin(tilePos.X * 2 + tilePos.Y - j * 0.45f + (float)Main.time / 20f) * 0.15f;
			// rotation += MathF.Sin(j / (float)height * MathHelper.Pi) * MathF.Sin(tilePos.X * 2 + tilePos.Y + (float)Main.time / 20f) * 6f / (height + 25);
			var tileLight = Lighting.GetColor(tilePos + new Point(0, -j));

			// 支持发光涂料
			tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y - j, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
			tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y - j, tile, type, 0, 0, tileLight);

			var origin = new Vector2(5, 18);

			var drawPos = drawCenterPos + lastOffset;
			var tileSpriteEffect = SpriteEffects.None;
			if (j > 1)
			{
				if (lastTileDis <= 0 || lastTileDis > 11)
				{
					spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
				}
			}
			else
			{
				if (j == 0)
				{
					int value = TileUtils.GetFixedRandomNumber(tile);
					frame = new Rectangle(18 * (value % 3), 34 * (value % 2), 18, 34);
					origin = new Vector2(08, 34);
					spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
				}
			}

			// Leaves in low joints
			if (lastTileDis <= 0)
			{
				if (j > 4 && jointStyle is 0 or 2)
				{
					Vector2 jointOffset = new Vector2(0, -14).RotatedBy(rotation);
					if (jointStyle == 2)
					{
						jointOffset = new Vector2(0, -6).RotatedBy(rotation);
					}
					var leftLeafFrame = new Rectangle(6, 138, 38, 30);
					var rightLeafFrame = new Rectangle(48, 136, 46, 30);
					Vector2 leaf_drawPos = jointOffset + drawPos;
					float sub_rot_Left = GetRotationLeaf(tileDrawing, tilePos.X - 1, tilePos.Y - j, rotation);
					float sub_rot_Right = GetRotationLeaf(tileDrawing, tilePos.X + 1, tilePos.Y - j, rotation);
					switch (TileUtils.GetFixedRandomNumber(tile) % 10)
					{
						case 0: // Left0
							spriteBatch.Draw(tex, leaf_drawPos, leftLeafFrame, tileLight, sub_rot_Left, new Vector2(38, 9), 1f, tileSpriteEffect, 0f);
							break;
						case 1: // Right0
							spriteBatch.Draw(tex, leaf_drawPos, rightLeafFrame, tileLight, sub_rot_Right, new Vector2(0, 9), 1f, tileSpriteEffect, 0f);
							break;
						case 2: // Left0, Right0
							spriteBatch.Draw(tex, leaf_drawPos, leftLeafFrame, tileLight, sub_rot_Left, new Vector2(38, 9), 1f, tileSpriteEffect, 0f);

							spriteBatch.Draw(tex, leaf_drawPos, rightLeafFrame, tileLight, sub_rot_Right, new Vector2(0, 9), 1f, tileSpriteEffect, 0f);
							break;
						case 3: // Left1, Right0
							leftLeafFrame = new Rectangle(8, 172, 40, 24);
							spriteBatch.Draw(tex, leaf_drawPos, leftLeafFrame, tileLight, sub_rot_Left, new Vector2(40, 5), 1f, tileSpriteEffect, 0f);

							spriteBatch.Draw(tex, leaf_drawPos, rightLeafFrame, tileLight, sub_rot_Right, new Vector2(0, 9), 1f, tileSpriteEffect, 0f);
							break;
						case 4: // Left1
							leftLeafFrame = new Rectangle(8, 172, 40, 24);
							spriteBatch.Draw(tex, leaf_drawPos, leftLeafFrame, tileLight, sub_rot_Left, new Vector2(40, 5), 1f, tileSpriteEffect, 0f);
							break;
						case 5: // Left1, Right1
							leftLeafFrame = new Rectangle(8, 172, 40, 24);
							spriteBatch.Draw(tex, leaf_drawPos, leftLeafFrame, tileLight, sub_rot_Left, new Vector2(40, 5), 1f, tileSpriteEffect, 0f);

							rightLeafFrame = new Rectangle(58, 170, 34, 24);
							spriteBatch.Draw(tex, leaf_drawPos, rightLeafFrame, tileLight, sub_rot_Right, new Vector2(0, 7), 1f, tileSpriteEffect, 0f);
							break;
						case 7: // Right1
							rightLeafFrame = new Rectangle(58, 170, 34, 24);
							spriteBatch.Draw(tex, leaf_drawPos, rightLeafFrame, tileLight, sub_rot_Right, new Vector2(0, 7), 1f, tileSpriteEffect, 0f);
							break;
						case 8: // Left0, Right1
							spriteBatch.Draw(tex, leaf_drawPos, leftLeafFrame, tileLight, sub_rot_Left, new Vector2(38, 9), 1f, tileSpriteEffect, 0f);

							rightLeafFrame = new Rectangle(58, 170, 34, 24);
							spriteBatch.Draw(tex, leaf_drawPos, rightLeafFrame, tileLight, sub_rot_Right, new Vector2(0, 7), 1f, tileSpriteEffect, 0f);
							break;
						default:
							break;
					}
				}
			}

			// Leaves in medium-low joints
			else if (lastTileDis is < 24 and > 18)
			{
				if (j > 4 && jointStyle is 0 or 2)
				{
					Vector2 jointOffset = new Vector2(0, -14).RotatedBy(rotation);
					if (jointStyle == 2)
					{
						jointOffset = new Vector2(0, -6).RotatedBy(rotation);
					}
					var leafFrame = new Rectangle(276, 184, 130, 54);
					Vector2 leaf_drawPos = jointOffset + drawPos;
					float sub_rot = GetRotationLeaf(tileDrawing, tilePos.X, tilePos.Y - j, rotation);
					switch (TileUtils.GetFixedRandomNumber(tile) % 2)
					{
						case 0:
							spriteBatch.Draw(tex, leaf_drawPos, leafFrame, tileLight, sub_rot, new Vector2(70, 24), 1f, tileSpriteEffect, 0f);
							break;
						case 1:
							spriteBatch.Draw(tex, leaf_drawPos, leafFrame, tileLight, sub_rot, new Vector2(70, 24), 1f, SpriteEffects.FlipHorizontally, 0f);
							break;
					}
				}
			}

			// Leaves in medium joints
			else if (lastTileDis is <= 18 and > 11)
			{
				if (j > 4 && jointStyle is 0 or 2)
				{
					Vector2 jointOffset = new Vector2(0, -14).RotatedBy(rotation);
					if (jointStyle == 2)
					{
						jointOffset = new Vector2(0, -6).RotatedBy(rotation);
					}
					var leafFrame = new Rectangle(274, 4, 144, 50);
					Vector2 anchorPos = new Vector2(72, 18);
					Vector2 leaf_drawPos = jointOffset + drawPos;
					float sub_rot = GetRotationLeaf_LargeLeaves(tileDrawing, tilePos.X, tilePos.Y - j, rotation);
					switch (TileUtils.GetFixedRandomNumber(tile) % 3)
					{
						case 0:
							break;
						case 1:
							leafFrame = new Rectangle(264, 58, 160, 54);
							anchorPos = new Vector2(82, 18);
							break;
						case 2:
							leafFrame = new Rectangle(278, 112, 156, 62);
							anchorPos = new Vector2(68, 18);
							break;
						default:
							break;
					}
					var leafFrame_front = leafFrame;
					leafFrame_front.X -= 172;
					SpriteEffects spriteEffects = SpriteEffects.None;
					if (TileUtils.GetFixedRandomNumber(tile) % 2 == 0)
					{
						spriteEffects = SpriteEffects.FlipHorizontally;
					}
					spriteBatch.Draw(tex, leaf_drawPos, leafFrame, tileLight, sub_rot, anchorPos, 1f, spriteEffects, 0f);
					spriteBatch.Draw(tex, leaf_drawPos, leafFrame_front, tileLight, sub_rot, anchorPos, 1f, spriteEffects, 0f);
				}
			}

			// Leaves in high joints
			if (lastTileDis is > 0 and <= 12)
			{
				Rectangle frame_top = new Rectangle(440, 0, 108, 188);
				Vector2 posLeft = drawPos + new Vector2(-54, 0).RotatedBy(rotation);
				Vector2 posRight = drawPos + new Vector2(54, 0).RotatedBy(rotation);
				float coordY = (lastTileDis - 1) / 11f * frame_top.Height / tex.Height;
				float coordX1 = 548f / tex.Width;
				float coordX2 = 440f / tex.Width;
				if (TileUtils.GetFixedRandomNumber(tilePos) % 2 == 1)
				{
					(coordX2, coordX1) = (coordX1, coordX2);
				}
				bamboo_top.Add(posRight, Lighting.GetColor((posRight + Main.screenPosition).ToTileCoordinates()), new Vector3(coordX2, coordY, 0));
				bamboo_top.Add(posLeft, Lighting.GetColor((posLeft + Main.screenPosition).ToTileCoordinates()), new Vector3(coordX1, coordY, 0));
			}

			lastOffset += new Vector2(0, -16).RotatedBy(rotation);
			lastRot = rotation;
		}
		if (bamboo_top.Count > 2)
		{
			spriteBatch.GraphicsDevice.Textures[0] = tex;
			spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bamboo_top.ToArray(), 0, bamboo_top.Count - 2);
		}
	}

	public float GetRotationLeaf(TileDrawing tileDrawing, int x, int y, float rotation)
	{
		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(x, y, 1, 1))
		{
			windCycle = tileDrawing.GetWindCycle(x, y, tileDrawing._sunflowerWindCounter);
		}
		float sub_highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(x, y, 1, 1, 140, 0.96f, 3, swapLoopDir: true);
		float sub_windCycle = windCycle + sub_highestWindGridPushComplex * 1.5f;
		float sub_rot = rotation + sub_windCycle * 0.25f;
		return sub_rot;
	}

	public float GetRotationLeaf_LargeLeaves(TileDrawing tileDrawing, int x, int y, float rotation)
	{
		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(x, y, 1, 1))
		{
			windCycle = tileDrawing.GetWindCycle(x, y, tileDrawing._sunflowerWindCounter);
		}
		float sub_highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(x, y, 1, 1, 140, 0.96f, 3, swapLoopDir: true);
		float sub_windCycle = windCycle + sub_highestWindGridPushComplex * 1.5f;
		float sub_rot = rotation + sub_windCycle * 0.05f;
		return sub_rot;
	}
}
