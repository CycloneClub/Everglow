using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Myth.TheTusk.Tiles;

public class StoneTusk : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			20,
		};
		TileObjectData.newTile.CoordinateWidth = 36;
		TileObjectData.addTile(Type);
		DustType = 191;
		var modTranslation = CreateMapEntryName();
		AddMapEntry(new Color(112, 83, 67), modTranslation);
		HitSound = SoundID.DD2_SkeletonHurt;
	}
	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = fail ? 1 : 3;
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
	}
	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<Items.Materials.StoneTusk>(), 3);
	}
	public override void PlaceInWorld(int i, int j, Item item)
	{
		short num = (short)Main.rand.Next(0, 12);
		Main.tile[i, j].TileFrameX = (short)(num * 36);
		Main.tile[i, j + 1].TileFrameX = (short)(num * 36);
		Main.tile[i, j + 2].TileFrameX = (short)(num * 36);
	}
}
