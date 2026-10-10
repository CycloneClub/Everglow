using Terraria.ObjectData;

namespace Everglow.Food.Tiles
{
	public class Hob_Silver : ModTile
	{
		public override void SetStaticDefaults()
		{
			// Properties
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileTable[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileSolidTop[Type] = true;
			TileID.Sets.RoomNeeds.CountsAsTable[Type] = true;

			DustType = DustID.Silver;

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.addTile(Type);

			// Etc
			AddMapEntry(new Color(200, 200, 200));
		}
	}
}
