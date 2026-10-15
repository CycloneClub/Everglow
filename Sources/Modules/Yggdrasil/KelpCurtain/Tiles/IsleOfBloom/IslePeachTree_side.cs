using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;

public class IslePeachTree_side : ModTile, ITileFluentlyDrawn, ITileOffsetOverScreenDrawn
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoFail[Type] = true;
		Main.tileAxe[Type] = true;

		DustType = ModContent.DustType<IslePeachTree_Sawdust>();

		// Placement - Standard Chandelier Setup Below
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 10;
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.DrawYOffset = 0;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty; // Clear out existing bottom anchor inherited from Style1x1 temporarily so that we don't have to set it to empty in each of the alternates.

		var SolidOrSolidSideAnchor1TilesLong = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = new Point16(0, 1);
		TileObjectData.newAlternate.AnchorLeft = SolidOrSolidSideAnchor1TilesLong;
		TileObjectData.newAlternate.Style = 1;
		TileObjectData.addAlternate(1);

		TileObjectData.newTile.Origin = new Point16(4, 1);
		TileObjectData.newTile.AnchorRight = SolidOrSolidSideAnchor1TilesLong;
		TileObjectData.addTile(Type);
		AnimationFrameHeight = 18;
		AddMapEntry(new Color(205, 101, 147));
		HitSound = SoundID.Dig;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = fail ? 1 : 3;
	}

	public override void RandomUpdate(int i, int j)
	{
		var petal = new PeachBlossom
		{
			Velocity = new Vector2(0, 0.5f).RotatedByRandom(Math.PI * 2),
			Active = true,
			Visible = true,
			Position = new Vector2(i, j).ToWorldCoordinates() + new Vector2(0, Main.rand.NextFloat()).RotatedByRandom(MathHelper.TwoPi) * 120 + new Vector2(0, -60),
			MaxTime = 3600,
			Scale = Main.rand.NextFloat(1f, 1.5f),
			Frame = Main.rand.Next(10),
			ai = new float[] { Main.rand.NextFloat(1f, 8f), -1 },
		};
		Ins.VFXManager.Add(petal);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (!ITileOffsetOverScreenDrawn.SpecialTilePositon.Contains(new Point(i, j)))
		{
			ITileOffsetOverScreenDrawn.SpecialTilePositon.Add(new Point(i, j));
		}
		Tile tile = TileUtils.SafeGetTile(i, j);
		if (tile.TileFrameX is 162 or 180)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
	{
		// Tile tile = TileUtils.SafeGetTile(i, j);
		// if (tile.TileFrameX is 72 or 90)
		// {
		// TileFluentDrawManager.AddFluentPoint(this, i, j);
		// }
		base.DrawEffects(i, j, spriteBatch, ref drawData);
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
		var tile = TileUtils.SafeGetTile(tilePos);
		Texture2D tex = ModAsset.IslePeachTree_side_tree.Value;

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[tilePos].TileColor;
		tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.IslePeachTree_side_tree_Path, Type, 1, paint, tileDrawing);
		tex ??= ModAsset.IslePeachTree_side_tree.Value;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
		{
			windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter) * 0.25f;
		}

		int totalPushTime = 140;
		float pushForcePerFrame = 0.96f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X - 20, tilePos.Y - 10, 20, 10, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		if (tile.TileFrameX == 180)
		{
			highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y - 10, 20, 10, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		}
		windCycle += highestWindGridPushComplex * 0.25f;
		float rotation = windCycle * 0.1f;
		if (!Main.gamePaused)
		{
			if (windCycle > 0.1f)
			{
				if (Main.rand.NextBool(2))
				{
					var petal = new PeachBlossom
					{
						Velocity = new Vector2(0, 0.5f).RotatedByRandom(Math.PI * 2),
						Active = true,
						Visible = true,
						Position = tilePos.ToWorldCoordinates() + new Vector2(0, Main.rand.NextFloat()).RotatedByRandom(MathHelper.TwoPi) * new Vector2(1, 0.1f) * 120 + new Vector2(-100, -80),
						MaxTime = 3600,
						Scale = Main.rand.NextFloat(0.8f, 1.2f),
						Frame = Main.rand.Next(10),
						ai = new float[] { Main.rand.NextFloat(1f, 8f), -1 },
					};
					if (tile.TileFrameX == 180)
					{
						petal.Position = tilePos.ToWorldCoordinates() + new Vector2(0, Main.rand.NextFloat()).RotatedByRandom(MathHelper.TwoPi) * new Vector2(1, 0.1f) * 120 + new Vector2(100, -80);
					}
					Ins.VFXManager.Add(petal);
				}
			}
		}

		var tileLight = Lighting.GetColor(tilePos);

		// 支持发光涂料
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, Type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y, tile, Type, 0, 0, tileLight);

		var frame = new Rectangle(0, 0, 412, 200);
		var origin = new Vector2(390, 179);
		if (tile.TileFrameX == 180)
		{
			frame = new Rectangle(416, 0, 412, 200);
			origin = new Vector2(21, 179);
		}

		TileUtils.VertexDraw_Grid(drawCenterPos, frame, origin, tex, spriteBatch, rotation);
	}

	int ITileOffsetOverScreenDrawn.TileOffsetScreenRange() => 480;
}
