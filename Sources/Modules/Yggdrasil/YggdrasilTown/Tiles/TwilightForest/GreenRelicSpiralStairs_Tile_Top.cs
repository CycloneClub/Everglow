using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public class GreenRelicSpiralStairs_Tile_Top : ModTile, ISceneTile
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

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 36, 78), name);
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => Commons.Utilities.AdministratorMode.Enabled;

	public override bool CanExplode(int i, int j) => Commons.Utilities.AdministratorMode.Enabled;

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		if (!Commons.Utilities.AdministratorMode.Enabled)
		{
			noBreak = true;
		}
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public void AddScene(int i, int j)
	{
		Tile tile = TileUtils.SafeGetTile(i, j);
		GreenRelicSpiralStairs_Top_foreground gRSSTf = new GreenRelicSpiralStairs_Top_foreground { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type, MaxDiatanceOutOfScreen = 1800, Origin = new Point(44, 3), Frame = new Rectangle(0, 0, 720, 80) };
		Ins.VFXManager.Add(gRSSTf);

		GreenRelicSpiralStairs_Top gRSST1 = new GreenRelicSpiralStairs_Top { Position = new Vector2(i - 15, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type, MaxDiatanceOutOfScreen = 1800, Origin = new Point(0, 4), Frame = new Rectangle(0, 0, 48, 96) };
		Ins.VFXManager.Add(gRSST1);

		GreenRelicSpiralStairs_Top gRSST2 = new GreenRelicSpiralStairs_Top { Position = new Vector2(i - 1, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type, MaxDiatanceOutOfScreen = 1800, Origin = new Point(0, 4), Frame = new Rectangle(48, 0, 48, 96) };
		Ins.VFXManager.Add(gRSST2);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}
}
