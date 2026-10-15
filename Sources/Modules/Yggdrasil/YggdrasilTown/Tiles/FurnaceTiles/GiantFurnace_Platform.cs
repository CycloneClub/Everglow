using Everglow.Commons.VFX.Scene;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class GiantFurnace_Platform : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileLighted[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileTable[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.Platforms[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
		AddMapEntry(new Color(84, 84, 84));
		MinPick = 1000000;
		DustType = DustID.Iron;
		AdjTiles = new int[] { TileID.Platforms };

		// Placement
		TileObjectData.newTile.CoordinateHeights = new[] { 16 };
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.StyleMultiplier = 27;
		TileObjectData.newTile.StyleWrapLimit = 27;
		TileObjectData.newTile.UsesCustomCanPlace = false;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		resetFrame = false;
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 648 && tile.TileFrameY == 144)
		{
			var scene = new GiantFurnace_SideLadder { Position = new Vector2(i, j) * 16 + new Vector2(-16, -4), Active = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(scene);
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}

	public override bool CanExplode(int i, int j) => false;

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
}
