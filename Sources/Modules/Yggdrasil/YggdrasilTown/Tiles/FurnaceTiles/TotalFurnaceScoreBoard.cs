using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class TotalFurnaceScoreBoard : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileSolid[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = false;
		MinPick = int.MaxValue;
		DustType = ModContent.DustType<Heatproof_Furniture_Dust>(); // You should set a kind of dust manually.

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.Width = 23;

		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(4, 0);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(84, 84, 84));
	}

	public override bool CanExplode(int i, int j) => false;

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override void MouseOver(int i, int j)
	{
		base.MouseOver(i, j);
	}

	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		{
			var scene = new FurnaceNumberAxis { Position = new Vector2(i, j) * 16, Active = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(scene);
		}
	}
}
