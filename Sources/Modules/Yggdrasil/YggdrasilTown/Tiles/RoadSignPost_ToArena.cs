using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.WorldGeneration;
using MathNet.Numerics.LinearAlgebra;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class RoadSignPost_ToArena : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileSolid[Type] = false;
		Main.tileWaterDeath[Type] = false;
		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.Height = 8;
		TileObjectData.newTile.Width = 1;

		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16, 16, 16, 16 };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(1, 1);
		TileObjectData.addTile(Type);
		DustType = DustID.WoodFurniture;
		HitSound = default;

		AddMapEntry(new Color(63, 51, 47));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
		return false;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return base.PreDraw(i, j, spriteBatch);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		//Tile tile = TileUtils.SafeGetTile(i, j);
		//if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		//{
		//	Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		//	if (Main.drawToScreen)
		//	{
		//		zero = Vector2.Zero;
		//	}
		//	var drawPos = new Point(i, j).ToWorldCoordinates() - Main.screenPosition + zero;
		//	Texture2D tileTex = ModAsset.RoadSignPost_ToArena.Value;
		//	var frame0 = new Rectangle(98, 0, 62, 16);
		//	var color0 = Lighting.GetColor(i + 3, j + 1);
		//	spriteBatch.Draw(tileTex, drawPos, frame0, color0, 0, new Vector2(0, 8), 1f, SpriteEffects.None, 0);

		//	var frame1 = new Rectangle(18, 10, 64, 16);
		//	var color1 = Lighting.GetColor(i - 3, j + 1);
		//	spriteBatch.Draw(tileTex, drawPos + new Vector2(0, 16), frame1, color1, 0, new Vector2(64, 8), 1f, SpriteEffects.None, 0);
		//}
		base.PostDraw(i, j, spriteBatch);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
	}

	public void AddScene(int i, int j)
	{
		Tile tile = TileUtils.SafeGetTile(i, j);
		if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		{
			RoadSignPost_ToArenaVFX rSV = new RoadSignPost_ToArenaVFX { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(rSV);
		}
	}
}
