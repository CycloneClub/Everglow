using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using static Everglow.Yggdrasil.WorldGeneration.YggdrasilWorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class StoneBridgeTile : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileSolid[Type] = true;
		Main.tileWaterDeath[Type] = false;
		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<StoneBridgeDust>();
		MinPick = int.MaxValue;
		AddMapEntry(new Color(64, 64, 61));
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
		return false;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
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
			var rightTile = TileUtils.SafeGetTile(i + 20, j);
			var leftTile = TileUtils.SafeGetTile(i - 1, j);
			if (rightTile.TileType == Type && rightTile.TileFrameY == 54)
			{
				StoneBridge_Fence_Right_Side sBF5 = new StoneBridge_Fence_Right_Side { Position = new Vector2(i, j - 3) * 16 + new Vector2(0, -16), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
				Ins.VFXManager.Add(sBF5);
			}
			else if (leftTile.TileType == Type && leftTile.TileFrameY == 18)
			{
				StoneBridge_Fence_Left_Side sBF6 = new StoneBridge_Fence_Left_Side { Position = new Vector2(i, j - 3) * 16 + new Vector2(0, -16), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
				Ins.VFXManager.Add(sBF6);
			}
			else
			{
				StoneBridge_Fence sBF = new StoneBridge_Fence { Position = new Vector2(i, j - 3) * 16 - new Vector2(0, 14), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
				Ins.VFXManager.Add(sBF);
			}
			StoneBridge_Foreground sBF2 = new StoneBridge_Foreground { Position = new Vector2(i, j - 3) * 16 - new Vector2(0, 4), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
			Ins.VFXManager.Add(sBF2);
			StoneBridge_Pier_Foreground sBF3 = new StoneBridge_Pier_Foreground { Position = new Vector2(i, j - 3) * 16 + new Vector2(-120, 316), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
			Ins.VFXManager.Add(sBF3);
			StoneBridge_Pier2_Foreground sBF4 = new StoneBridge_Pier2_Foreground { Position = new Vector2(i, j - 3) * 16 + new Vector2(-4, 630), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
			Ins.VFXManager.Add(sBF4);
		}
		if (tile.TileFrameX == 0 && tile.TileFrameY == 18)
		{
			StoneBridge_Leftside_Foreground sBF2 = new StoneBridge_Leftside_Foreground { Position = new Vector2(i, j - 3) * 16 + new Vector2(-272, -4), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
			Ins.VFXManager.Add(sBF2);
		}
		if (tile.TileFrameX == 0 && tile.TileFrameY == 54)
		{
			StoneBridge_Rightside_Foreground sBF2 = new StoneBridge_Rightside_Foreground { Position = new Vector2(i, j - 3) * 16 + new Vector2(0, -4), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
			Ins.VFXManager.Add(sBF2);
			StoneBridge_Pier_Foreground sBF3 = new StoneBridge_Pier_Foreground { Position = new Vector2(i, j - 3) * 16 + new Vector2(-120, 316), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
			Ins.VFXManager.Add(sBF3);
			StoneBridge_Pier2_Foreground sBF4 = new StoneBridge_Pier2_Foreground { Position = new Vector2(i, j - 3) * 16 + new Vector2(-4, 630), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
			Ins.VFXManager.Add(sBF4);
		}
		if (tile.TileFrameX == 0 && tile.TileFrameY == 72)
		{
			StoneBridge_Middleside_Foreground sBF2 = new StoneBridge_Middleside_Foreground { Position = new Vector2(i, j - 3) * 16 + new Vector2(0, -116), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
			Ins.VFXManager.Add(sBF2);
			StoneBridge_Pier_Foreground sBF3 = new StoneBridge_Pier_Foreground { Position = new Vector2(i + 7, j - 10) * 16 + new Vector2(-112, 316), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
			Ins.VFXManager.Add(sBF3);
			StoneBridge_Pier2_Foreground sBF4 = new StoneBridge_Pier2_Foreground { Position = new Vector2(i + 7, j - 10) * 16 + new Vector2(4, 630), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<StoneBridgeTile>() };
			Ins.VFXManager.Add(sBF4);
		}
	}
}
