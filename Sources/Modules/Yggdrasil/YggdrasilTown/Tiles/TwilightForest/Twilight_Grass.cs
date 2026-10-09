using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public class Twilight_Grass : ModTile
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
		DustType = ModContent.DustType<TwilightGrassDust>();
		AddMapEntry(new Color(6, 85, 107));
		HitSound = SoundID.Grass;
	}

	public override void RandomUpdate(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX < 108)
		{
			tile.TileFrameX += 108;
		}
		base.RandomUpdate(i, j);
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		Tile tile = Main.tile[i, j];
		short frameXStyle = (short)(Main.rand.Next(6) * 18);
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
