using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Commons.TileHelper;
using Everglow.Myth.TheFirefly.Dusts;
using Terraria.GameContent.Drawing;

namespace Everglow.Myth.TheFirefly.Tiles;

public class FluorescentTree : ModTile, ITileFluentlyDrawn
{
	/// <summary>
	/// 挂藤质点
	/// </summary>
	public static MassSpringContainer FluorescentTreeVineMassSpringSystem = new MassSpringContainer();
	public static EulerSolver FluorescentTreeVineEulerSolver = new EulerSolver(8);
	public Dictionary<int, Dictionary<Point, Rope>> StyleVines = new Dictionary<int, Dictionary<Point, Rope>>();

	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileLavaDeath[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileBlockLight[Type] = false;
		Main.tileLighted[Type] = true;
		Main.tileAxe[Type] = true;
		Main.tileNoAttach[Type] = false;

		TileID.Sets.IsATreeTrunk[Type] = true;
		AddMapEntry(new Color(51, 26, 58), CreateMapEntryName());
		DustType = ModContent.DustType<FluorescentTreeDust>();
		AdjTiles = new int[] { Type };
		for (int i = 0; i < 5; i++)
		{
			StyleVines.Add(i, new Dictionary<Point, Rope>());
		}
	}

	public override void AnimateTile(ref int frame, ref int frameCounter)
	{
		if (Main.gamePaused)
		{
			return;
		}
		FluorescentTreeVineMassSpringSystem = new MassSpringContainer();
		foreach (var style in StyleVines.Values)
		{
			foreach (var vine in style.Values)
			{
				FluorescentTreeVineMassSpringSystem.AddMassSpringMesh(vine);
			}
		}
		FluorescentTreeVineEulerSolver.Step(FluorescentTreeVineMassSpringSystem, 1);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<Items.GlowWood>());
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Tile tile = Main.tile[i, j];
		switch (tile.TileFrameY)
		{
			default:
				return;

			case 2: // 树冠
				for (int k = 0; k < 5; k++)
				{
					var style = StyleVines[k];
					if (!style.ContainsKey(new Point(i, j)))
					{
						Vector2 offset = Vector2.zeroVector;
						switch (k)
						{
							case 0:
								offset = new Vector2(23, -16);
								break;
							case 1:
								offset = new Vector2(37, -57);
								break;
							case 2:
								offset = new Vector2(-18, -25);
								break;
							case 3:
								offset = new Vector2(-35, -85);
								break;
							case 4:
								offset = new Vector2(-51, -60);
								break;
						}
						if (tile.TileFrameX == 1)
						{
							switch (k)
							{
								case 0:
									offset = new Vector2(14, -20);
									break;
								case 1:
									offset = new Vector2(26, -40);
									break;
								case 2:
									offset = new Vector2(-19, -65);
									break;
								case 3:
									offset = new Vector2(-45, -65);
									break;
								case 4:
									offset = new Vector2(42, -64);
									break;
							}
						}
						Rope rope = Rope.Create_Fixed_StartPos(offset + new Vector2(i, j) * 16, Main.rand.Next(2, 9), 10f, 2f, 20f);
						style.Add(new Point(i, j), rope);
					}
				}
				break;
			case 4: // 左树枝
				if (tile.TileFrameX == 0)
				{
					var style = StyleVines[0];
					if (!style.ContainsKey(new Point(i, j)))
					{
						Vector2 offset = new Vector2(-12, 11);
						Rope rope = Rope.Create_Fixed_StartPos(offset + new Vector2(i, j) * 16, Main.rand.Next(1, 4), 10f, 2f, 20f);
						style.Add(new Point(i, j), rope);
					}
				}
				if (tile.TileFrameX == 2)
				{
					var style = StyleVines[0];
					if (!style.ContainsKey(new Point(i, j)))
					{
						Vector2 offset = new Vector2(-15, 18);
						Rope rope = Rope.Create_Fixed_StartPos(offset + new Vector2(i, j) * 16, Main.rand.Next(1, 4), 10f, 2f, 20f);
						style.Add(new Point(i, j), rope);
					}
				}
				break;
			case 5: // 右树枝
				if (tile.TileFrameX == 0)
				{
					var style = StyleVines[0];
					if (!style.ContainsKey(new Point(i, j)))
					{
						Vector2 offset = new Vector2(9, 12);
						Rope rope = Rope.Create_Fixed_StartPos(offset + new Vector2(i, j) * 16, Main.rand.Next(1, 4), 10f, 2f, 20f);
						style.Add(new Point(i, j), rope);
					}
				}
				break;
		}
	}

	public Vector2 GetStyleOffset(int style, int styleFrameX = 0, int styleFrameY = 2)
	{
		Vector2 offset = Vector2.zeroVector;
		if (styleFrameY == 2)
		{
			switch (style)
			{
				case 0:
					offset = new Vector2(23, -16);
					break;
				case 1:
					offset = new Vector2(37, -57);
					break;
				case 2:
					offset = new Vector2(-18, -25);
					break;
				case 3:
					offset = new Vector2(-35, -85);
					break;
				case 4:
					offset = new Vector2(-51, -60);
					break;
			}
			if (styleFrameX == 1)
			{
				switch (style)
				{
					case 0:
						offset = new Vector2(14, -20);
						break;
					case 1:
						offset = new Vector2(26, -40);
						break;
					case 2:
						offset = new Vector2(-19, -65);
						break;
					case 3:
						offset = new Vector2(-45, -65);
						break;
					case 4:
						offset = new Vector2(42, -64);
						break;
				}
			}
		}
		if (styleFrameY == 4)
		{
			if (styleFrameX == 0)
			{
				offset = new Vector2(-12, 11);
			}
			if (styleFrameX == 2)
			{
				offset = new Vector2(-15, 18);
			}
		}
		if (styleFrameY == 5)
		{
			offset = new Vector2(9, 12);
		}
		return offset;
	}

	public override bool CanDrop(int i, int j)
	{
		for (int x = 0; x < 6; x++)
		{
			Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustType, 0, 0, 0, default, Main.rand.NextFloat(0.5f, 1f));
		}
		var tile = Main.tile[i, j];
		if (tile.TileFrameY == 2)
		{
			for (int x = 0; x < 12; x++)
			{
				Gore.NewGore(null, new Vector2(i * 16 - 32, j * 16 - 120) + new Vector2(Main.rand.Next(80), Main.rand.Next(120)), new Vector2(0, Main.rand.NextFloat(0f, 1f)).RotatedByRandom(6.283), ModContent.GoreType<FireflyTree_Leaf>(), Main.rand.NextFloat(0.65f, 1.45f));
			}
			for (int x = 0; x < 64; x++)
			{
				Dust.NewDust(new Vector2(i * 16 - 40, j * 16 - 100), 96, 96, DustType, 0, 0, 0, default, Main.rand.NextFloat(0.5f, 1f));
			}
		}
		if (tile.TileFrameY > 3)
		{
			Gore.NewGore(null, new Vector2(i * 16, j * 16) + new Vector2(Main.rand.Next(16), Main.rand.Next(16)), new Vector2(0, Main.rand.NextFloat(0f, 1f)).RotatedByRandom(6.283), ModContent.GoreType<FireflyTree_Leaf>(), Main.rand.NextFloat(0.65f, 1.45f));
		}
		return false;
	}

	private void Shake(int i, int j, int frameY)
	{
		if (Main.rand.NextBool(7))
		{
			if (frameY == 2)
			{
				for (int x = 0; x < 12; x++)
				{
					Gore.NewGore(null, new Vector2(i * 16 - 32, j * 16 - 120) + new Vector2(Main.rand.Next(80), Main.rand.Next(120)), new Vector2(0, Main.rand.NextFloat(0f, 1f)).RotatedByRandom(6.283), ModContent.GoreType<FireflyTree_Leaf>(), Main.rand.NextFloat(0.65f, 1.45f));
				}
			}
			if (frameY > 3)
			{
				Gore.NewGore(null, new Vector2(i * 16, j * 16) + new Vector2(Main.rand.Next(16), Main.rand.Next(16)), new Vector2(0, Main.rand.NextFloat(0f, 1f)).RotatedByRandom(6.283), ModContent.GoreType<FireflyTree_Leaf>(), Main.rand.NextFloat(0.65f, 1.45f));
			}
		}
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		// TODO 处理手动的Drop调用
		int Dy = -1; // 向上破坏的自变化Y坐标
		if (!fail)
		{
			// 以下是破坏的特效,比如落叶
			if (Main.tile[i, j].TileFrameY < 4)
			{
				Tile tileLeft;
				Tile tileRight;
				tileLeft = Main.tile[i - 1, j];
				if (tileLeft.TileType == Type)
				{
					Shake(i - 1, j, tileLeft.TileFrameY);
					CanDrop(i - 1, j);
				}
				tileRight = Main.tile[i + 1, j];
				if (tileRight.TileType == Type)
				{
					Shake(i + 1, j, tileRight.TileFrameY);
					CanDrop(i + 1, j);
				}
				while (Main.tile[i, j + Dy].HasTile && Main.tile[i, j + Dy].TileType == Type && Dy > -100)
				{
					Shake(i, j + Dy, Main.tile[i, j + Dy].TileFrameY);
					CanDrop(i, j + Dy);

					tileLeft = Main.tile[i - 1, j + Dy];
					tileRight = Main.tile[i + 1, j + Dy];
					if (tileLeft.TileType == Type)
					{
						if (tileLeft.TileFrameY == 2)
						{
							break;
						}

						Shake(i - 1, j + Dy, tileLeft.TileFrameY);
						CanDrop(i - 1, j + Dy);
					}
					if (tileRight.TileType == Type)
					{
						if (tileRight.TileFrameY == 2)
						{
							break;
						}

						Shake(i + 1, j + Dy, tileRight.TileFrameY);
						CanDrop(i + 1, j + Dy);
					}
					Dy -= 1;
				}
				Dy = -1; // 向上破坏的自变化Y坐标
				tileLeft = Main.tile[i - 1, j];
				if (tileLeft.TileType == Type)
				{
					tileLeft.HasTile = false;
				}
				tileRight = Main.tile[i + 1, j];
				if (tileRight.TileType == Type)
				{
					tileRight.HasTile = false;
				}
				while (Main.tile[i, j + Dy].TileType == Type && Dy > -100)
				{
					Tile baseTile = Main.tile[i, j + Dy];

					baseTile.HasTile = false;

					tileLeft = Main.tile[i - 1, j + Dy];
					tileRight = Main.tile[i + 1, j + Dy];
					if (tileLeft.TileType == Type)
					{
						tileLeft.HasTile = false;
					}

					if (tileRight.TileType == Type)
					{
						tileRight.HasTile = false;
					}
					Dy -= 1;
				}
			}
		}
		else
		{
			Tile tileLeft;
			Tile tileRight;
			while (Main.tile[i, j + Dy].HasTile && Main.tile[i, j + Dy].TileType == Type && Dy > -100)
			{
				Shake(i, j + Dy, Main.tile[i, j + Dy].TileFrameY);
				tileLeft = Main.tile[i - 1, j + Dy];
				tileRight = Main.tile[i + 1, j + Dy];
				if (tileLeft.TileType == Type)
				{
					if (tileLeft.TileFrameY == 2)
					{
						break;
					}
					Shake(i - 1, j + Dy, tileLeft.TileFrameY);
				}
				if (tileRight.TileType == Type)
				{
					if (tileRight.TileFrameY == 2)
					{
						break;
					}
					Shake(i + 1, j + Dy, tileRight.TileFrameY);
				}
				Dy -= 1;
			}
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		TileFluentDrawManager.AddFluentPoint(this, i, j);
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		DrawTreePiece(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// Draw a piece of moss
	/// </summary>
	private void DrawTreePiece(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[tilePos];
		ushort type = tile.TileType;
		var frame = new Rectangle(tile.TileFrameX * 26, 2, 26, 16);
		var offset = new Vector2(0, 20);
		var stability = 0f;
		var origin = new Vector2(frame.Width * 0.5f, frame.Height);
		switch (tile.TileFrameY)
		{
			default:
				return;
			case 0: // 树桩
				frame = new Rectangle(tile.TileFrameX * 74, 180, 74, 24);
				origin = new Vector2(frame.Width * 0.5f, frame.Height);
				break;
			case 1: // 树干
				break;
			case 2: // 树冠
				offset = new Vector2(0, 24);
				frame = new Rectangle(tile.TileFrameX * 150, 46, 150, 132);
				origin = new Vector2(frame.Width * 0.5f, frame.Height);
				stability = 0.2f;
				Lighting.AddLight(tilePos.ToWorldCoordinates(), new Vector3(0.0f, 0.2f, 0.6f));
				break;
			case 3: // 粗树干
				offset = new Vector2(0, 28);
				frame = new Rectangle(tile.TileFrameX * 50, 20, 50, 24);
				origin = new Vector2(frame.Width * 0.5f, frame.Height);
				break;
			case 4: // 左树枝
				offset = new Vector2(10, 32);
				stability = 0.4f;
				frame = new Rectangle(tile.TileFrameX * 34, 240, 34, 32);
				origin = new Vector2(frame.Width, frame.Height * 0.5f);
				break;
			case 5: // 右树枝
				offset = new Vector2(-10, 32);
				stability = 0.4f;
				frame = new Rectangle(tile.TileFrameX * 34, 206, 34, 32);
				origin = new Vector2(0, frame.Height * 0.5f);
				break;
		}

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[tilePos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.FluorescentTree_Path, type, 1, paint, tileDrawing);
		tex ??= ModAsset.FluorescentTree.Value;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
		{
			windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);
		}

		int totalPushTime = 140;
		float pushForcePerFrame = 0.96f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;
		float rotation = windCycle * 0.21f * stability;

		var tileLight = Lighting.GetColor(tilePos);

		// 支持发光涂料
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y, tile, type, 0, 0, tileLight);

		var tileSpriteEffect = SpriteEffects.None;
		for (int k = 0; k < 5; k++)
		{
			if (GetStyleOffset(k, tile.TileFrameX, tile.TileFrameY) == Vector2.zeroVector)
			{
				break;
			}
			var style = StyleVines[k];
			if (style.ContainsKey(tilePos))
			{
				Rope rope = style[tilePos];
				var masses = rope.Masses;
				for (int i = 0; i < masses.Length - 1; i++)
				{
					Mass thisMass = masses[i];
					Mass nextMass = masses[i + 1];
					if (i == 0)
					{
						thisMass.Position = tilePos.ToWorldCoordinates() + new Vector2(8) + GetStyleOffset(k, tile.TileFrameX, tile.TileFrameY).RotatedBy(rotation);
					}
					float windCycleVine = 0;
					if (tileDrawing.InAPlaceWithWind((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), 1, 1))
					{
						windCycleVine = tileDrawing.GetWindCycle((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), tileDrawing._sunflowerWindCounter);
					}

					float highestWindGridPushComplexVine = tileDrawing.GetHighestWindGridPushComplex((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
					windCycleVine += highestWindGridPushComplexVine * 3;
					if (!Main.gamePaused)
					{
						rope.ApplyForceSpecial(i, new Vector2(windCycleVine * 0.05f, 0.2f * thisMass.Value));
						if (i == masses.Length - 2)
						{
							rope.ApplyForceSpecial(i + 1, new Vector2(windCycleVine * 0.05f, 0.2f * nextMass.Value));
						}
					}

					// 支持发光涂料
					Color tileLightVine;
					tileLightVine = Lighting.GetColor((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f));
					Vector2 toNextMass = nextMass.Position - thisMass.Position;
					Vector2 drawPos = thisMass.Position - Main.screenPosition;
					var frameVine = new Rectangle(18 * ((tilePos.X + tilePos.Y + k) % 3), 274 + (int)((k * 18) % 32), 18, 22);

					spriteBatch.Draw(tex, drawPos, frameVine, tileLightVine, toNextMass.ToRotation() + MathHelper.PiOver2 * 3, new Vector2(9f, 0), 1f, tileSpriteEffect, 0);
					frameVine.Y += 324;
					spriteBatch.Draw(tex, drawPos, frameVine, new Color(1f, 1f, 1f, 0), toNextMass.ToRotation() + MathHelper.PiOver2 * 3, new Vector2(9f, 0), 1f, tileSpriteEffect, 0);
				}
			}
		}
		spriteBatch.Draw(tex, drawCenterPos + offset, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
		frame.Y += 324;
		spriteBatch.Draw(tex, drawCenterPos + offset, frame, new Color(1f, 1f, 1f, 0), rotation, origin, 1f, tileSpriteEffect, 0f);
	}
}
