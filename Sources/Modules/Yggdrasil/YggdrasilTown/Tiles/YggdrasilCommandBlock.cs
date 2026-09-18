using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.Background;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;
using SubworldLibrary;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class YggdrasilCommandBlock : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileNoAttach[Type] = false;
		Main.tileWaterDeath[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.newTile.Origin = new(0, 0);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new int[2];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(0, 0);

		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<UnionMarblePost_Dust_Khaki>();
		MinPick = int.MaxValue;
		AddMapEntry(new Color(96, 96, 96));
	}

	public override void RandomUpdate(int i, int j)
	{
		if (SubworldSystem.Current is YggdrasilWorld)
		{
			YggdrasilTownCentralSystem.CheckNPC(ModContent.NPCType<Guard_of_YggdrasilTown>());
		}
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		{
			BackgroundSystem bgSystem = ModContent.GetInstance<BackgroundSystem>();
			if (!bgSystem.HasBgSlide("Everglow.Yggdrasil.YggdrasilTown.Background.YggdrasilTown_Construct"))
			{
				AddBackground(bgSystem, i, j);
			}
		}
	}

	public void AddBackground(BackgroundSystem bgSystem, int i, int j)
	{
		List<Vector2> polygon = new List<Vector2>();
		Vector2 centerPosWorld = new Point(i, j).ToWorldCoordinates() + new Vector2(260, -464);
		polygon.Add(centerPosWorld + new Vector2(-210, 0) * 16);
		polygon.Add(centerPosWorld + new Vector2(-210, -30) * 16);
		polygon.Add(centerPosWorld + new Vector2(-150, -60) * 16);
		polygon.Add(centerPosWorld + new Vector2(-50, -89) * 16);
		polygon.Add(centerPosWorld + new Vector2(-20, -89) * 16);
		polygon.Add(centerPosWorld + new Vector2(60, -50) * 16);
		polygon.Add(centerPosWorld + new Vector2(120, -65) * 16);
		polygon.Add(centerPosWorld + new Vector2(170, -60) * 16);
		polygon.Add(centerPosWorld + new Vector2(210, -20) * 16);
		polygon.Add(centerPosWorld + new Vector2(210, 0) * 16);
		List<Point> bgArea = TileUtils.GetPolygonAreaOfTilePos(polygon);

		YggdrasilTown_Construct ytc = new YggdrasilTown_Construct();
		ytc.WorldAnchor = centerPosWorld + new Vector2(0, 704);
		ytc.BgTiles = bgArea;
		ytc.TileAnchor = new Point(i, j);
		bgSystem.AddBackgroundSlide(ytc);

		GiantFurnace_Construct gfc = new GiantFurnace_Construct();
		gfc.WorldAnchor = centerPosWorld + new Vector2(181, 63) * 16 + new Vector2(4, 24);
		gfc.BgTiles = TileUtils.GetAABBAreaOfTile((int)centerPosWorld.X / 16 + 134, (int)centerPosWorld.Y / 16 + 32, 92, 60);
		gfc.TileAnchor = new Point(i, j);
		bgSystem.AddBackgroundSlide(gfc);

		GiantFurnace_Construct_far gfcf = new GiantFurnace_Construct_far();
		gfcf.WorldAnchor = centerPosWorld + new Vector2(181, 63) * 16 + new Vector2(-264, -164);
		gfcf.BgTiles = TileUtils.GetAABBAreaOfTile((int)centerPosWorld.X / 16 + 106, (int)centerPosWorld.Y / 16 + 34, 112, 60);
		gfcf.TileAnchor = new Point(i, j);
		bgSystem.AddBackgroundSlide(gfcf);

		GiantFurnace_Construct_sky gfcs = new GiantFurnace_Construct_sky();
		gfcs.WorldAnchor = centerPosWorld + new Vector2(0, -240);
		gfcs.BgTiles = TileUtils.GetAABBAreaOfTile((int)centerPosWorld.X / 16 + 106, (int)centerPosWorld.Y / 16 + 4, 112, 90);
		gfcs.TileAnchor = new Point(i, j);
		bgSystem.AddBackgroundSlide(gfcs);

		FurnaceScoreShop fSS = new FurnaceScoreShop();
		fSS.WorldAnchor = centerPosWorld + new Vector2(-156, 920);
		fSS.BgTiles = TileUtils.GetAABBAreaOfTile((int)centerPosWorld.X / 16 + 100, (int)centerPosWorld.Y / 16 + 78, 20, 14);
		fSS.TileAnchor = new Point(i, j);
		bgSystem.AddBackgroundSlide(fSS);
	}
}
