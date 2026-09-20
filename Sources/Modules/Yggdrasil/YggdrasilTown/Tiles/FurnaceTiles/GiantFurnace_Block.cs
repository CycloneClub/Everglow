using Everglow.Commons.VFX.Scene;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class GiantFurnace_Block : ModTile, ISceneTile
{
	public override string Texture => ModAsset.GiantFurnace_Mod;

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileBlendAll[Type] = true;
		Main.tileBlockLight[Type] = true;

		DustType = DustID.Iron;
		HitSound = default;
		MinPick = 1000000;
		AddMapEntry(new Color(57, 55, 52));
	}

	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 126 && tile.TileFrameY == 144)
		{
			GiantFurnace_TopFence scene = new GiantFurnace_TopFence { Position = new Vector2(i, j) * 16 + new Vector2(-18, -26), Active = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(scene);
		}

		if (tile.TileFrameX == 360 && tile.TileFrameY == 522)
		{
			GiantFurnace_MiddleFence scene = new GiantFurnace_MiddleFence { Position = new Vector2(i, j) * 16 + new Vector2(-10, -18), Active = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(scene);
		}
	}

	public override bool CanExplode(int i, int j) => false;

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return base.PreDraw(i, j, spriteBatch);
	}
}
