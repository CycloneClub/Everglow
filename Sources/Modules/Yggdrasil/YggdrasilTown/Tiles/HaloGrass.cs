using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class HaloGrass : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileCut[Type] = true;
		Main.tileLighted[Type] = true;
		Main.SettingsEnabled_TilesSwayInWind = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
		   34,
		};
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.DrawYOffset = -16;
		TileObjectData.addTile(Type);
		TileID.Sets.SwaysInWindBasic[Type] = true;
		DustType = ModContent.DustType<HaloGrass_dust>();
		AddMapEntry(new Color(73, 182, 255));
		HitSound = SoundID.Grass;
	}

	public override void RandomUpdate(int i, int j, bool underground)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX < 108)
		{
			tile.TileFrameX += 108;
		}
		base.RandomUpdate(i, j, underground);
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		Tile tile = Main.tile[i, j];
		short frameXStyle = (short)(Main.rand.Next(6) * 18);
		tile.TileFrameX = frameXStyle;
		base.PlaceInWorld(i, j, item);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = 0.36f;
		g = 0.62f;
		b = 0.9f;
		base.ModifyLight(i, j, ref r, ref g, ref b);
	}
}
