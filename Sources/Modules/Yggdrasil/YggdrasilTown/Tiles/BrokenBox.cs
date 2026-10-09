using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class BrokenBox : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;

		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<BoxWoodDust_LampWood>();
		AddMapEntry(new Color(102, 70, 51));
	}
	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{

		base.KillMultiTile(i, j, frameX, frameY);
	}
	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		noItem = true;
		base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
	}
	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameY == 18)
		{
			if (tile.TileFrameX == 36)
			{
			}
			if (tile.TileFrameX == 54)
			{
			}
		}
		return base.PreDraw(i, j, spriteBatch);
	}
}
