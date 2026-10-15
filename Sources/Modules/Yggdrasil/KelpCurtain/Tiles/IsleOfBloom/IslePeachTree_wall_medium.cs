using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;

public class IslePeachTree_wall_medium : ModTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileWaterDeath[Type] = false;
		Main.tileAxe[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.AnchorWall = true;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<IslePeachTree_Sawdust>();
		AddMapEntry(new Color(205, 101, 147));
		HitSound = SoundID.Dig;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		TileFluentDrawManager.AddFluentPoint(this, i, j);
		return false;
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
		Texture2D tex;

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[tilePos].TileColor;
		tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.IslePeachTree_wall_medium_Path, Type, 1, paint, tileDrawing);
		tex ??= ModAsset.IslePeachTree_wall_medium.Value;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
		{
			windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter) * 0.25f;
		}

		int totalPushTime = 140;
		float pushForcePerFrame = 0.96f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X - 4, tilePos.Y - 1, 8, 3, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex * 0.25f;
		float rotation = windCycle * 0.2f;

		var tileLight = Lighting.GetColor(tilePos);

		// 支持发光涂料
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, Type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y, tile, Type, 0, 0, tileLight);

		var frame = new Rectangle(0, 26, 182, 142);
		var origin = new Vector2(88, 140);
		var offset = new Vector2(0, 12);
		var tileBelow = TileUtils.SafeGetTile(tilePos + new Point(0, 1));
		if (tileBelow.IsHalfBlock)
		{
			offset.Y += 8;
		}
		switch (TileUtils.GetFixedRandomNumber(tilePos, 2))
		{
			case 0:
				frame = new Rectangle(12, 6, 138, 46);
				origin = new Vector2(91, 36);
				break;
			case 1:
				frame = new Rectangle(166, 12, 130, 40);
				origin = new Vector2(68, 34);
				break;
		}
		if (rotation > 0.02f)
		{
			GenerateDust(tilePos.X, tilePos.Y, origin, frame);
		}
		TileUtils.VertexDraw_Grid(drawCenterPos + offset, frame, origin, tex, spriteBatch, rotation);
	}
	public void GenerateDust(int i, int j, Vector2 origin, Rectangle frame)
	{
		if (!Main.gamePaused)
		{
			if (Main.rand.NextBool(12))
			{
				Point tilePos = new Point(i, j);
				int style = TileUtils.GetFixedRandomNumber(tilePos, 2);
				var petal = new PeachBlossom
				{
					Velocity = new Vector2(0, 0.5f).RotatedByRandom(Math.PI * 2),
					Active = true,
					Visible = true,
					Position = tilePos.ToWorldCoordinates(),
					MaxTime = 3600,
					Scale = Main.rand.NextFloat(0.8f, 1.2f),
					Frame = Main.rand.Next(10),
					ai = new float[] { Main.rand.NextFloat(1f, 8f), -1 },
				};
				petal.Position += new Vector2(Main.rand.NextFloat(frame.Width), Main.rand.NextFloat(frame.Height * 0.3f) + frame.Height * 0.3f) - origin;
				Ins.VFXManager.Add(petal);
			}
		}
	}
}
