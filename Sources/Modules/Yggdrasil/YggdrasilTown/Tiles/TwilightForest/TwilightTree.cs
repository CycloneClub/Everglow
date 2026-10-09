using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Terraria.GameContent.Drawing;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public class TwilightTree : ModTile, ITileFluentlyDrawn
{
	/// <summary>
	/// 挂藤质点
	/// </summary>
	public static MassSpringContainer TwilightTreeVineMassSpringSystem = new MassSpringContainer();
	public static EulerSolver TwilightTreeVineEulerSolver = new EulerSolver(8);
	public Dictionary<int, Dictionary<Point, Rope>> StyleVines = new Dictionary<int, Dictionary<Point, Rope>>();

	public override void AnimateTile(ref int frame, ref int frameCounter)
	{
		if (Main.gamePaused)
		{
			return;
		}
		TwilightTreeVineMassSpringSystem = new MassSpringContainer();
		foreach (var style in StyleVines.Values)
		{
			foreach (var vine in style.Values)
			{
				TwilightTreeVineMassSpringSystem.AddMassSpringMesh(vine);
			}
			foreach (var poses in style.Keys)
			{
				Tile tile = TileUtils.SafeGetTile(poses);
				if (tile.TileType != Type)
				{
					style.Remove(poses);
				}
			}
		}
		TwilightTreeVineEulerSolver.Step(TwilightTreeVineMassSpringSystem, 1);
	}

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
		var modTranslation = Language.GetText("Mods.Everglow.MapEntry.TwilightTree");
		AddMapEntry(new Color(58, 53, 50), modTranslation);
		DustType = ModContent.DustType<TwilightTreeDust>();
		AdjTiles = new int[] { Type };
		for (int i = 0; i < 4; i++)
		{
			StyleVines.Add(i, new Dictionary<Point, Rope>());
		}
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameY == 3)
		{
			for (int k = 0; k < 4; k++)
			{
				var style = StyleVines[k];
				if (!style.ContainsKey(new Point(i, j)))
				{
					Vector2 offset = GetStyleOffset(k);

					var rope = Rope.Create_Fixed_StartPos(offset + new Vector2(i, j) * 16, Main.rand.Next(10, 20), 10f, 2f, 20f);
					style.Add(new Point(i, j), rope);
				}
			}
		}
	}

	public Vector2 GetStyleOffset(int style)
	{
		Vector2 offset = Vector2.zeroVector;
		switch (style)
		{
			case 0:
				{
					offset = new Vector2(-128, -66);
					break;
				}
			case 1:
				{
					offset = new Vector2(-112, -64);
					break;
				}
			case 2:
				{
					offset = new Vector2(54, -44);
					break;
				}
			case 3:
				{
					offset = new Vector2(134, -84);
					break;
				}
		}
		return offset;
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<TwilightEucalyptusWood_Item>());
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
				Gore.NewGore(null, new Vector2(i * 16 - 32, j * 16 - 120) + new Vector2(Main.rand.Next(80), Main.rand.Next(120)), new Vector2(0, Main.rand.NextFloat(0f, 1f)).RotatedByRandom(6.283), ModContent.GoreType<TwilightTree_Leaf>(), Main.rand.NextFloat(0.65f, 1.45f));
			}
			for (int x = 0; x < 64; x++)
			{
				Dust.NewDust(new Vector2(i * 16 - 40, j * 16 - 100), 96, 96, DustType, 0, 0, 0, default, Main.rand.NextFloat(0.5f, 1f));
			}
		}
		if (tile.TileFrameY > 3)
		{
			Gore.NewGore(null, new Vector2(i * 16, j * 16) + new Vector2(Main.rand.Next(16), Main.rand.Next(16)), new Vector2(0, Main.rand.NextFloat(0f, 1f)).RotatedByRandom(6.283), ModContent.GoreType<TwilightTree_Leaf>(), Main.rand.NextFloat(0.65f, 1.45f));
		}
		return true;
	}

	private void Shake(int i, int j, int frameY)
	{
		if (Main.rand.NextBool(7))
		{
			if (frameY == 2)
			{
				for (int x = 0; x < 12; x++)
				{
					Gore.NewGore(null, new Vector2(i * 16 - 32, j * 16 - 120) + new Vector2(Main.rand.Next(80), Main.rand.Next(120)), new Vector2(0, Main.rand.NextFloat(0f, 1f)).RotatedByRandom(6.283), ModContent.GoreType<TwilightTree_Leaf>(), Main.rand.NextFloat(0.65f, 1.45f));
				}
			}
			if (frameY > 3)
			{
				Gore.NewGore(null, new Vector2(i * 16, j * 16) + new Vector2(Main.rand.Next(16), Main.rand.Next(16)), new Vector2(0, Main.rand.NextFloat(0f, 1f)).RotatedByRandom(6.283), ModContent.GoreType<TwilightTree_Leaf>(), Main.rand.NextFloat(0.65f, 1.45f));
			}
		}
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		Tile tile = Main.tile[i, j];
		if (effectOnly)
		{
			return;
		}

		if (!fail)// 判定为已破碎
		{
			bool isLeft = tile.TileFrameY == 1;
			noItem = true;
			if (isLeft)
			{
				KillToTop(i, j);
			}
			else if (tile.TileFrameY == 2)
			{
				KillToTop(i - 1, j);
			}
			else if (tile.TileFrameY == 0)
			{
				KillToTop(i, j);
			}
			else if (tile.TileFrameY == -1)
			{
				var tileRight = Main.tile[i + 1, j];
				if (tileRight.TileType == Type)
				{
					KillToTop(i, j);
				}
				else
				{
					KillToTop(i - 1, j);
				}
			}
		}
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
	}

	/// <summary>
	/// fill the left tile coords.
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	public void KillToTop(int i, int j)
	{
		int breakingY = j; // 向上破坏的自变化Y坐标
		while (true)
		{
			Tile tile = Main.tile[i, breakingY];
			Tile tileRight = Main.tile[i + 1, breakingY];
			breakingY--;
			if ((tile.HasTile && tile.TileType == Type) || breakingY == j)
			{
				WorldGen.KillTile(i, breakingY, false, true, false);
				WorldGen.KillTile(i + 1, breakingY, false, true, false);
				tile.ClearEverything();
				tileRight.ClearEverything();
				for (int x = 0; x < 2; x++)
				{
					Dust.NewDust(new Vector2(i, breakingY) * 16, 16, 16, DustType);
					Dust.NewDust(new Vector2(i + 1, breakingY) * 16, 16, 16, DustType);
				}
				Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, breakingY), i * 16, breakingY * 16, 16, 16, new Item(ModContent.ItemType<TwilightEucalyptusWood_Item>(), 1));
				Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, breakingY), i * 16 + 16, breakingY * 16, 16, 16, new Item(ModContent.ItemType<TwilightEucalyptusWood_Item>(), 1));
			}
			else
			{
				break;
			}
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Texture2D treeTexture = ModAsset.TwilightTree.Value;
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Tile tile = Main.tile[i, j];
		int Width;
		int Height = 16;
		int TexCoordY;
		int OffsetY = 20;
		int OffsetX = 8;
		Color color = Lighting.GetColor(i, j);
		switch (tile.TileFrameY)
		{
			default: // 别的表示空,最好写-1
				return false;
			case 0: // 树桩
				Width = 116;
				Height = 42;
				TexCoordY = 298;
				break;
			case 1: // 左树干
				Width = 16;
				TexCoordY = 264;
				OffsetX += -8;
				break;
			case 2: // 右树干
				Width = 16;
				TexCoordY = 280;
				OffsetX += -8;
				break;
			case 3: // 树冠
				TileFluentDrawManager.AddFluentPoint(this, i, j);
				return false;
		}
		var origin = new Vector2(Width / 2f, Height);
		spriteBatch.Draw(treeTexture, new Vector2(i * 16 + OffsetX + 8, j * 16 + OffsetY) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX * Width, TexCoordY, Width, Height), color, 0, origin, 1, SpriteEffects.None, 0);
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		for (int k = 0; k < 4; k++)
		{
			var style = StyleVines[k];
			if (style.ContainsKey(pos))
			{
				Rope rope = style[pos];
				DrawVine(rope, pos, spriteBatch, tileDrawing, k);
			}
		}
	}

	public void DrawVine(Rope vine, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing, int style, Color color = default)
	{
		// 回声涂料
		if (!TileDrawing.IsVisible(Main.tile[pos]))
		{
			return;
		}

		var tile = Main.tile[pos];
		ushort type = tile.TileType;
		int paint = Main.tile[pos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.TwilightTree_Path, type, 1, paint, tileDrawing);
		tex ??= ModAsset.TwilightTree.Value;
		var tileSpriteEffect = SpriteEffects.None;
		float treeRot = tileDrawing.GetHighestWindGridPushComplex(pos.X, pos.Y - 3, 3, 3, 150, 0.06f, 4, swapLoopDir: true);

		var masses = vine.Masses;
		for (int i = 0; i < masses.Length - 1; i++)
		{
			Mass thisMass = masses[i];
			Mass nextMass = masses[i + 1];
			if (i == 0)
			{
				thisMass.Position = pos.ToVector2() * 16f + new Vector2(16, 24) + GetStyleOffset(style).RotatedBy(treeRot);
			}
			int totalPushTime = 80;
			float pushForcePerFrame = 1.26f;
			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), tileDrawing._sunflowerWindCounter);
			}

			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f), 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex;
			if (!Main.gamePaused)
			{
				vine.ApplyForceSpecial(i, new Vector2(windCycle / 4.0f, 0.4f * thisMass.Value));
				if (i == masses.Length - 2)
				{
					vine.ApplyForceSpecial(i + 1, new Vector2(windCycle / 4.0f, 0.4f * nextMass.Value));
				}
			}

			// 支持发光涂料
			Color tileLight;
			if (color != default)
			{
				tileLight = color;
			}
			else
			{
				tileLight = Lighting.GetColor((int)((thisMass.Position.X - 8) / 16f), (int)((thisMass.Position.Y - 8) / 16f));
			}
			Vector2 toNextMass = nextMass.Position - thisMass.Position;
			Vector2 drawPos = thisMass.Position - Main.screenPosition;

			if (i == masses.Length - 2)
			{
				spriteBatch.Draw(tex, drawPos, new Rectangle(0, 352, 6, 14), tileLight, toNextMass.ToRotation() + MathHelper.PiOver2 * 3, new Vector2(3f, 0), 1f, tileSpriteEffect, 0);
				spriteBatch.Draw(tex, drawPos, new Rectangle(0, 352, 6, 14), new Color(1f, 1f, 1f, 0), toNextMass.ToRotation() + MathHelper.PiOver2 * 3, new Vector2(3f, 0), 1f, tileSpriteEffect, 0);
				Lighting.AddLight(drawPos + Main.screenPosition, new Vector3(0f, 0.4f, 0.7f));
			}
			else
			{
				spriteBatch.Draw(tex, drawPos, new Rectangle(0, 342, 6, 6), tileLight, toNextMass.ToRotation() + MathHelper.PiOver2 * 3, new Vector2(3f, 0), new Vector2(1f, toNextMass.Length() / 6f), tileSpriteEffect, 0);
			}
		}

		Color tileLight2;
		if (color != default)
		{
			tileLight2 = color;
		}
		else
		{
			tileLight2 = Lighting.GetColor(pos);
		}

		spriteBatch.Draw(tex, pos.ToVector2() * 16 + new Vector2(16, 24) - Main.screenPosition, new Rectangle(0, 0, 410, 262), tileLight2, treeRot, new Vector2(205, 262), 1, SpriteEffects.None, 0);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameY == 3)
		{
			r = 0.03f;
			g = 0.124f;
			b = 0.124f;
		}
		base.ModifyLight(i, j, ref r, ref g, ref b);
	}
}
