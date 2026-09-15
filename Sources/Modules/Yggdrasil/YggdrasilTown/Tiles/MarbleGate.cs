using Everglow.Commons.VFX.Scene;
using Everglow.SubSpace;
using Everglow.SubSpace.Tiles;
using static Everglow.Yggdrasil.WorldGeneration.YggdrasilWorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class MarbleGate : RoomDoorTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileSolid[Type] = true;
		Main.tileWaterDeath[Type] = false;
		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = true;
		DustType = DustID.Stone;
		MinPick = int.MaxValue;
		AddMapEntry(new Color(0, 0, 0));
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
			MarbleGate_BackgroundTile mGBT = new MarbleGate_BackgroundTile { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			MarbleGate_UnionMarblePost mGUMP = new MarbleGate_UnionMarblePost { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(mGBT);
			Ins.VFXManager.Add(mGUMP);
		}
	}

	public override bool RightClick(int i, int j)
	{
		return false;
	}
}
