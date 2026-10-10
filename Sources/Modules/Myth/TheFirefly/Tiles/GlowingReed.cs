using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles;

public class GlowingReed : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileCut[Type] = true;
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
		DustType = DustID.SpookyWood;
		var modTranslation = CreateMapEntryName();
		AddMapEntry(new Color(11, 11, 160), modTranslation);
		HitSound = SoundID.Grass;
	}

	public override void RandomUpdate(int i, int j, bool underground)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX < 180)
		{
			tile.TileFrameX += 90;
		}
		base.RandomUpdate(i, j, underground);
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		Tile tile = Main.tile[i, j];
		short frameXStyle = (short)(Main.rand.Next(5) * 18);
		tile.TileFrameX = frameXStyle;
		base.PlaceInWorld(i, j, item);
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield break;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return true;
	}
}
