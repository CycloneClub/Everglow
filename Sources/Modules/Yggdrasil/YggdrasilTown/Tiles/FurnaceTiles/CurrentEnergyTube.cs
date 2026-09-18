using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class CurrentEnergyTube : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileSolid[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileBlendAll[Type] = true;
		MinPick = int.MaxValue;
		DustType = ModContent.DustType<Heatproof_Furniture_Dust>();

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 12;
		TileObjectData.newTile.Width = 1;

		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new int[12];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(0, 11);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(240, 115, 0));
	}

	public override void NearbyEffects(int i, int j, bool closer) => base.NearbyEffects(i, j, closer);

	public override bool CanExplode(int i, int j) => false;

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		{
			var scene = new CurrentEnergyTube_LavaBar { Position = new Vector2(i, j) * 16, Active = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(scene);
		}
	}
}
